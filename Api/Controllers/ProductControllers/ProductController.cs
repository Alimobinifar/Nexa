using Application.Interfaces.Products;
using Contracts.Cmmon.Dtos.Product;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers.ProductController
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductDto product)
        {
            var response = await _productService.CreateProduct(product);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct(
            [FromBody] UpdateProductDto product)
        {
            var response = await _productService.UpdateProduct(product);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("Delete/{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var response = await _productService.DeleteProduct(productId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.ProductsList();

            return StatusCode(response.StatusCode, response);
        }
    }


}
