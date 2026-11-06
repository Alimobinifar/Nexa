using Application.Interfaces.IWarehouseInventory;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.WarehouseInventoryControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseInventoryController : ControllerBase
    {
        private readonly IWarehouseInventoryService _inventoryService;

        public WarehouseInventoryController(
            IWarehouseInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            var response = await _inventoryService.GetAllInventories();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetInventoryById(int id)
        {
            var response = await _inventoryService.GetInventoryById(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId)
        {
            var response = await _inventoryService.GetInventoryByProductId(productId);

            return StatusCode(response.StatusCode, response);
        }
    }
}
