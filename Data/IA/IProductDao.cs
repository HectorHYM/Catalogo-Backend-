using CatalogoBackend.Models.Entities;

namespace CatalogoBackend.Data.IA
{
    public interface IProductDao
    {
        Task<IEnumerable<Product>> GetProductsAsync();
    }
}
