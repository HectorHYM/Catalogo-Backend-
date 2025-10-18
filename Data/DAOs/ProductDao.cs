using CatalogoBackend.Data.IA;
using CatalogoBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoBackend.Data.DAOs
{
    public class ProductDao : IProductDao
    {
        private readonly AppDbContext _db;

        public ProductDao(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _db.Products.ToListAsync();
        }
    }
}
