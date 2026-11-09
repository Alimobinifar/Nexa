using Application.Interfaces.SaleChannels;
using Contracts.Cmmon.Dtos.SaleChannels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.SalesChannels
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesChannelController : ControllerBase
    {
        private readonly ISalesChannelService _salesChannelService;

        public SalesChannelController(
            ISalesChannelService salesChannelService)
        {
            _salesChannelService = salesChannelService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSalesChannel(
            [FromBody] CreateSalesChannelDto dto)
        {
            var response =
                await _salesChannelService.CreateSalesChannel(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSalesChannel(
            [FromBody] UpdateSalesChannelDto dto)
        {
            var response =
                await _salesChannelService.UpdateSalesChannel(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSalesChannel(int id)
        {
            var response =
                await _salesChannelService.DeleteSalesChannel(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSalesChannels()
        {
            var response =
                await _salesChannelService.GetAllSalesChannels();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSalesChannelById(int id)
        {
            var response =
                await _salesChannelService.GetSalesChannelById(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSalesChannels(
            [FromQuery] string searchTerm)
        {
            var response =
                await _salesChannelService.SearchSalesChannels(searchTerm);

            return StatusCode(response.StatusCode, response);
        }
    }
}
