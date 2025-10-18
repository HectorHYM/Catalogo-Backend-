using CatalogoBackend.Services.IA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Controllers
{
    [ApiController]
    [Route("/products")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        //* Endpoint para obtener todos los productos existentes
        [HttpGet("get-products")]
        [Authorize]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var res = await _productService.GetProductsAsync();

                switch (res.Code)
                {
                    case ResponseCode.Ok:
                        return Ok(res);
                    case ResponseCode.NoContent:
                        return NoContent();
                    case ResponseCode.BadRequest:
                        return BadRequest(res);
                    case ResponseCode.NotFound:
                        return NotFound();
                    case ResponseCode.ServerError:
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError);

                }
            }
            catch(InvalidOperationException ex)
            {
                //^LOG
                _logger.LogError("Error al cargar los productos: {ex}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
