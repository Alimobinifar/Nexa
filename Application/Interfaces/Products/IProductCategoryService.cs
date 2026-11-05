using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Products
{
    public interface IProductCategoryService
    {
        Task<ResponseDto<ProductCategoryDto>> CreateProductCategory(
             CreateProductCategoryDto category);

        Task<ResponseDto<ProductCategoryDto>> UpdateProductCategory(
            UpdateProductCategoryDto category);

        Task<ResponseDto<bool>> DeleteProductCategory(int categoryId);

        Task<ResponseDto<IEnumerable<ProductCategoryDto>>> GetAllCategories();

        Task<ResponseDto<ProductCategoryDto>> GetCategoryById(int categoryId);

        Task<ResponseDto<IEnumerable<ProductCategoryDto>>> GetCategoriesByParentId(int parentId);
    }
}
