using Application.Interfaces.ProductAllocations;
using Contracts.Cmmon.Dtos.ProductChannelAllocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ProductChannelAllocationControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductChannelAllocationController : ControllerBase
    {
        private readonly IProductChannelAllocationService _service;

        public ProductChannelAllocationController(
            IProductChannelAllocationService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateAllocation(
            [FromBody] CreateProductChannelAllocationDto dto)
        {
            var response = await _service.CreateAllocation(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAllocation(
            [FromBody] UpdateProductChannelAllocationDto dto)
        {
            var response = await _service.UpdateAllocation(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAllocation(int id)
        {
            var response = await _service.DeleteAllocation(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAllocations()
        {
            var response = await _service.GetAllAllocations();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAllocationById(int id)
        {
            var response = await _service.GetAllocationById(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetAllocationsByProductId(
            int productId)
        {
            var response =
                await _service.GetAllocationsByProductId(productId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("channel/{channelId:int}")]
        public async Task<IActionResult> GetAllocationsByChannelId(
            int channelId)
        {
            var response =
                await _service.GetAllocationsByChannelId(channelId);

            return StatusCode(response.StatusCode, response);
        }
    }
}
