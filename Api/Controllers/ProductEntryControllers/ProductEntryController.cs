using Application.Interfaces.ProductEntries;
using Contracts.Cmmon.Dtos.ProductEntries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ProductEntryControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductEntryController : ControllerBase
    {
        private readonly IProductEntryService _productEntryService;

        public ProductEntryController(IProductEntryService productEntryService)
        {
            _productEntryService = productEntryService;
        }
        
        // Create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProductEntry(
            [FromBody] CreateProductEntryDto dto)
        {
            var response = await _productEntryService.CreateProductEntry(dto);

            return StatusCode(response.StatusCode, response);
        }

        // Update
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProductEntry(
            [FromBody] UpdateProductEntryDto dto)
        {
            var response = await _productEntryService.UpdateProductEntry(dto);

            return StatusCode(response.StatusCode, response);
        }

        // Delete
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProductEntry(int id)
        {
            var response = await _productEntryService.DeleteProductEntry(id);

            return StatusCode(response.StatusCode, response);
        }

        // Get All
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProductEntries()
        {
            var response = await _productEntryService.GetAllProductEntries();

            return StatusCode(response.StatusCode, response);
        }

        // Get By Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductEntryById(int id)
        {
            var response = await _productEntryService.GetProductEntryById(id);

            return StatusCode(response.StatusCode, response);
        }
    }
}
