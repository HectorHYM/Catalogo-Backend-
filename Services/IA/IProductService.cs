using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Services.IA
{
    public interface IProductService
    {
        Task<GeneralResponse<IEnumerable<ProductDto>>> GetProductsAsync();
    }
}
