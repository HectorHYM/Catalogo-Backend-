using Microsoft.EntityFrameworkCore;
using CatalogoBackend.Models.Entities;

namespace CatalogoBackend.Data
{
    public class AppDbContext : DbContext
    {
        //* Contructor que recibe las opciones de configuración del contexto de la base de datos (Program.cs)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //*DbSet para cada una de las entidades del modelo
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        //*Configuración del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("catalog"); //* Establece el esquema por defecto para las tablas

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "tbl_users", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "tbl_user_role_check", sql: "role IN ('client', 'admin')"); //* Se configura una restricción de chequeo para la columna Role, permitiendo solo los valores 'client' o 'admin'
                }); //* Configura el nombre de la tabla para la entidad User

                entity.HasKey(e => e.Id); //* Configura la clave primaria de la entidad User
                entity.Property(e => e.Id).HasColumnName("id").IsRequired().HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd(); //* Configura la columna Id como NOT NULL, con valor por defecto generado automáticamente (UUID)
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired(); //* Configura la longitud máxima y que no puede ser nulo de la propiedad Name
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(30).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("idx_tbl_users_username"); //* Crea un índice único para la columna Username
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("idx_tbl_users_email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(200).IsRequired(false);
                entity.Property(e => e.Role).HasColumnName("roleID").HasMaxLength(20).IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd(); //* Configura la columna CreatedAt como NOT NULL, con valor por defecto de la fecha y hora actual
                entity.Property(e => e.CreatedById).HasColumnName("created_by");
                entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedUsers).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_users_created_by_fkey"); //* Se configura la relación con la entidad User para CreatedBy, permitiendo que al eliminar un usuario, el campo se establezca a NULL (ON DELETE SET NULL) y estableciendo la colección de usuarios creados por este usuario.
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.UpdatedById).HasColumnName("updated_by");
                entity.HasOne(e => e.UpdatedBy).WithMany(u => u.UpdatedUsers).HasForeignKey(e => e.UpdatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_users_updated_by_fkey");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable(name: "tbl_products", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "tbl_products_price_check", sql: "price >= 0"); //* Se configura una restricción de chequeo para la columna Price, asegurando que el precio no sea negativo
                    table.HasCheckConstraint(name: "tbl_products_stock_check", sql: "stock >= 0"); //* Se configura una restricción de chequeo para la columna Stock, asegurando que el stock no sea negativo
                }); //* Configura el nombre de la tabla para la entidad Product

                entity.HasKey(e => e.Id); //* Configura la clave primaria de la entidad Product
                entity.Property(e => e.Id).HasColumnName("id").IsRequired().HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)").HasDefaultValue(0).IsRequired(); //* Configura la columna Price como decimal con 10 dígitos en total y 2 decimales
                entity.Property(e => e.Stock).HasColumnName("stock").HasDefaultValue(0).IsRequired(); //* Configura la columna Stock como NOT NULL, con valor por defecto de 0
                entity.Property(e => e.ImgUrl).HasColumnName("img_url").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
                entity.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
                entity.HasOne(e => e.Category).WithMany(c => c.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("tbl_products_category_fkey"); //* Eliminación estricta, es decir no se pueden eliminar categorías hasta que no haya ningún producto dentro de esta, todos los productos deben pertenecer a una categoría.
                entity.HasIndex(e => e.CategoryId).HasDatabaseName("idx_tbl_products_category_id");
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd(); //* Configura la columna CreatedAt como NOT NULL, con valor por defecto de la fecha y hora actual.
                entity.Property(e => e.CreatedById).HasColumnName("created_by");
                entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedProducts).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_products_created_by_fkey"); //* Se configura la relación con la entidad User para CreatedBy, permitiendo que al eliminar un usuario, el campo se establezca a NULL (ON DELETE SET NULL) y estableciendo la colección de usuarios creados por este usuario.
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.UpdatedById).HasColumnName("updated_by");
                entity.HasOne(e => e.UpdatedBy).WithMany(u => u.UpdatedProducts).HasForeignKey(e => e.UpdatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_products_updated_by_fkey");
            });

            modelBuilder.Entity<Token>(entity =>
            {
                entity.ToTable(name: "tbl_tokens", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "chk_type", sql: "type IN ('access', 'recover')"); //* Se configura una restricción de chequeo para la columna Type, permitiendo solo los valores 'access' o 'recover'
                    table.HasCheckConstraint(name: "chk_expires", sql: "expires_at > created_at"); //* Se configura una restricción de chequeo para la columna ExpiresAt, asegurando que la fecha de expiración sea posterior a la fecha de creación
                });

                entity.HasKey(e => e.Id); //* Se configura la clave primaria de la entidad Token
                entity.Property(e => e.Id).HasColumnName("id").IsRequired().HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.HasOne(e => e.User).WithMany(u => u.UserTokens).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("tbl_tokens_user_id_fkey"); //* Se configura la relación con la entidad User, estableciendo que al eliminar un usuario, se eliminen sus tokens asociados (ON DELETE CASCADE)
                entity.HasIndex(e => e.UserId).HasDatabaseName("idx_tbl_tokens_userid");
                entity.Property(e => e.TokenValue).HasColumnName("token_hash").IsRequired().HasMaxLength(64); //* Se configura la columna TokenValue como NOT NULL, con una longitud máxima de 255 caracteres
                entity.HasIndex(e => e.TokenValue).IsUnique().HasDatabaseName("idx_tbl_tokens_token_hash");
                entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(40);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd().IsRequired();
                entity.Property(e => e.ExpiresAt).HasColumnName("expires_at").HasColumnType("timestamp with time zone").IsRequired();
                entity.Property(e => e.UsedAt).HasColumnName("used_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable(name: "tbl_categories", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "chk_name", sql: "name IN ('all', 'electronics', 'home', 'clothes', 'sports', 'beauty', 'games', 'toys', 'healthy', 'automotive', 'books', 'yard', 'tools', 'pets', 'children', 'jewelry', 'others')");
                }); 

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasDefaultValue("others").HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable(name: "tbl_favorites");

                // PK compuesta
                entity.HasKey(e => new { e.UserId, e.ProductId }).HasName("pk_tbl_favorites");
                // Columnas
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
                // FKs
                entity.HasOne(e => e.User).WithMany(u => u.FavoritesProducts).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("tbl_favorites_user_id_fkey");
                entity.HasOne(e => e.Product).WithMany(p => p.FavoritedBy).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("tbl_favorites_product_id_fkey");
                // Indexes
                entity.HasIndex(e => e.UserId).HasDatabaseName("idx_tbl_favorites_user_id");
                entity.HasIndex(e => e.ProductId).HasDatabaseName("idx_tbl_favorites_product_id");
            });
        }
    }
}