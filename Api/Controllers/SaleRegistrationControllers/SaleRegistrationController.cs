using Application.Interfaces.SalesRegistration;
using Contracts.Cmmon.Dtos.SaleRegistration;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.SaleRegistrationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleRegistrationController : ControllerBase
    {
        private readonly ISaleRegistrationService _saleService;

        public SaleRegistrationController(
            ISaleRegistrationService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSaleRegistration(
            [FromBody] CreateSaleRegistrationDto dto)
        {
            var response =
                await _saleService.CreateSaleRegistration(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateSaleRegistration(
            [FromBody] UpdateSaleRegistrationDto dto)
        {
            var response =
                await _saleService.UpdateSaleRegistration(dto);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteSaleRegistration(int id)
        {
            var response =
                await _saleService.DeleteSaleRegistration(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSaleRegistrations()
        {
            var response =
                await _saleService.GetAllSaleRegistrations();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetSaleRegistrationById(int id)
        {
            var response =
                await _saleService.GetSaleRegistrationById(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetByProductId/{productId:int}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var response =
                await _saleService.GetByProductId(productId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetByChannelId/{channelId:int}")]
        public async Task<IActionResult> GetByChannelId(int channelId)
        {
            var response =
                await _saleService.GetByChannelId(channelId);

            return StatusCode(response.StatusCode, response);
        }
    }
}
