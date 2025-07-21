using Microsoft.EntityFrameworkCore;
using Catalogo_Backend_.Models;

namespace Catalogo_Backend_.Data
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

        //*Configuración del modelo de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("catalog"); //* Establece el esquema por defecto para las tablas

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "tbl_users", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "tbl_user_ck_role", sql: "role IN ('client', 'admin')"); //* Se configura una restricción de chequeo para la columna Role, permitiendo solo los valores 'client' o 'admin'
                }); //* Configura el nombre de la tabla para la entidad User

                entity.HasKey(e => e.Id); //* Configura la clave primaria de la entidad User
                entity.Property(e => e.Id).HasColumnName("id").IsRequired().HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd(); //* Configura la columna Id como NOT NULL, con valor por defecto generado automáticamente (UUID)
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired(); //* Configura la longitud máxima y que no puede ser nulo de la propiedad Name
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(30).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique().HasDatabaseName("tbl_users_ix_username"); //* Crea un índice único para la columna Username
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("tbl_users_ix_email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd(); //* Configura la columna CreatedAt como NOT NULL, con valor por defecto de la fecha y hora actual
                entity.Property(e => e.CreatedById).HasColumnName("created_by");
                entity.HasOne(e => e.CreatedBy).WithMany(u => u.CreatedUsers).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_users_fk_created_by"); //* Se configura la relación con la entidad User para CreatedBy, permitiendo que al eliminar un usuario, el campo se establezca a NULL (ON DELETE SET NULL) y estableciendo la colección de usuarios creados por este usuario.
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.UpdatedById).HasColumnName("updated_by");
                entity.HasOne(e => e.UpdatedBy).WithMany(u => u.UpdatedUsers).HasForeignKey(e => e.UpdatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_users_fk_updated_by");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable(name: "tbl_products", buildAction: table =>
                {
                    table.HasCheckConstraint(name: "tbl_products_ck_price", sql: "price >= 0"); //* Se configura una restricción de chequeo para la columna Price, asegurando que el precio no sea negativo
                    table.HasCheckConstraint(name: "tbl_products_ck_stock", sql: "stock >= 0"); //* Se configura una restricción de chequeo para la columna Stock, asegurando que el stock no sea negativo
                }); //* Configura el nombre de la tabla para la entidad Product

                entity.HasKey(e => e.Id); //* Configura la clave primaria de la entidad Product
                entity.Property(e => e.Id).HasColumnName("id").IsRequired().HasDefaultValueSql("gen_random_uuid()").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)").HasDefaultValue(0).IsRequired(); //* Configura la columna Price como decimal con 10 dígitos en total y 2 decimales
                entity.Property(e => e.Stock).HasColumnName("stock").HasDefaultValue(0).IsRequired(); //* Configura la columna Stock como NOT NULL, con valor por defecto de 0
                entity.Property(e => e.ImgUrl).HasColumnName("img_url").HasMaxLength(255);
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
                entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd(); //* Configura la columna CreatedAt como NOT NULL, con valor por defecto de la fecha y hora actual
                entity.Property(e => e.CreatedById).HasColumnName("created_by");
                entity.HasOne(e => e.CreatedBy).WithMany(p => p.CreatedProducts).HasForeignKey(e => e.CreatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_products_fk_created_by"); //* Se configura la relación con la entidad User para CreatedBy, permitiendo que al eliminar un usuario, el campo se establezca a NULL (ON DELETE SET NULL) y estableciendo la colección de usuarios creados por este usuario.
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone").ValueGeneratedOnAddOrUpdate();
                entity.Property(e => e.UpdatedById).HasColumnName("updated_by");
                entity.HasOne(e => e.UpdatedBy).WithMany(p => p.UpdatedProducts).HasForeignKey(e => e.UpdatedById).OnDelete(DeleteBehavior.SetNull).HasConstraintName("tbl_products_fk_updated_by");
            });
        }
    }
}