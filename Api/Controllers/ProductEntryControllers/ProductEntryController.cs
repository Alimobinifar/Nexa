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
        [HttpPost]
        public async Task<IActionResult> CreateProductEntry(
            [FromBody] CreateProductEntryDto dto)
        {
            var response = await _productEntryService.CreateProductEntry(dto);

            return StatusCode(response.StatusCode, response);
        }

        // Update
        [HttpPut]
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
        [HttpGet]
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

        // Get By Product
        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetProductEntriesByProductId(int productId)
        {
            var response = await _productEntryService
                .GetProductEntriesByProductId(productId);

            return StatusCode(response.StatusCode, response);
        }

        // Get By Supplier
        [HttpGet("supplier/{supplierId:int}")]
        public async Task<IActionResult> GetProductEntriesBySupplierId(int supplierId)
        {
            var response = await _productEntryService
                .GetProductEntriesBySupplierId(supplierId);

            return StatusCode(response.StatusCode, response);
        }

        // Get By Date Range
        [HttpGet("date-range")]
        public async Task<IActionResult> GetProductEntriesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = await _productEntryService
                .GetProductEntriesByDateRange(startDate, endDate);

            return StatusCode(response.StatusCode, response);
        }
    }
}
