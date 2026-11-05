using Application.Interfaces.Suppliers;
using Contracts.Cmmon.Dtos.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.SupplierControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateSupplier(
            [FromBody] CreateSupplierDto supplier)
        {
            var response = await _supplierService.CreateSupplier(supplier);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateSupplier(
            [FromBody] UpdateSupplierDto supplier)
        {
            var response = await _supplierService.UpdateSupplier(supplier);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("Delete/{supplierId:int}")]
        public async Task<IActionResult> DeleteSupplier(int supplierId)
        {
            var response = await _supplierService.DeleteSupplier(supplierId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSuppliers()
        {
            var response = await _supplierService.GetAllSuppliers();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetById/{supplierId:int}")]
        public async Task<IActionResult> GetSupplierById(int supplierId)
        {
            var response = await _supplierService.GetSupplierById(supplierId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchSuppliers(
            [FromQuery] string keyword)
        {
            var response = await _supplierService.SearchSuppliers(keyword);

            return StatusCode(response.StatusCode, response);
        }
    }

}
