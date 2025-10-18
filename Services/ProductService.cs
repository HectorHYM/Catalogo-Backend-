using CatalogoBackend.Data.IA;
using CatalogoBackend.Models.DTOs;
using CatalogoBackend.Models.Responses;
using CatalogoBackend.Services.IA;

namespace CatalogoBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductDao _productDao;

        public ProductService(IProductDao productDao)
        {
            _productDao = productDao;
        }

        public async Task<GeneralResponse<IEnumerable<ProductDto>>> GetProductsAsync()
        {
            var products = await _productDao.GetProductsAsync();
            if (products == null) return GeneralResponse<IEnumerable<ProductDto>>.Fail(null, "Parece que aún no hay productos, vuelva más tarde!.", ResponseCode.NotFound);
            var dtos = products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price, Stock = p.Stock, ImgUrl = p.ImgUrl, IsActive = p.IsActive  });
            return GeneralResponse<IEnumerable<ProductDto>>.Ok(dtos, "Productos cargados correctamente.");
        }
    }
}
