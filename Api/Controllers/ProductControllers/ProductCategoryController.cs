using Application.Interfaces.Products;
using Contracts.Cmmon.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.ProductwController
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _categoryService;

        public ProductCategoryController(
            IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateProductCategory(
            [FromBody] CreateProductCategoryDto category)
        {
            var response = await _categoryService.CreateProductCategory(category);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProductCategory(
            [FromBody] UpdateProductCategoryDto category)
        {
            var response = await _categoryService.UpdateProductCategory(category);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("Delete/{categoryId:int}")]
        public async Task<IActionResult> DeleteProductCategory(int categoryId)
        {
            var response = await _categoryService.DeleteProductCategory(categoryId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetById/{categoryId:int}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var response = await _categoryService.GetCategoryById(categoryId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetByParentId/{parentId:int}")]
        public async Task<IActionResult> GetCategoriesByParentId(int parentId)
        {
            var response = await _categoryService.GetCategoriesByParentId(parentId);

            return StatusCode(response.StatusCode, response);
        }
    }

}
