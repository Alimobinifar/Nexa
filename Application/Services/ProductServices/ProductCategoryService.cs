using Application.Interfaces.Products;
using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Domain.Entities.Product;
using Domain.Entities.Products;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.ProductServices
{

    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ProductCategory> _categoryRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly ILogger<ProductCategoryService> _logger;

        public ProductCategoryService(
            IUnitOfWork unitOfWork,
            IGenericRepository<ProductCategory> categoryRepository,
            IGenericRepository<Product> productRepository,
            ILogger<ProductCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductCategoryDto>> CreateProductCategory(
            CreateProductCategoryDto category)
        {
            try
            {
                if (category == null)
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category data is required.", 400);

                if (string.IsNullOrWhiteSpace(category.CategoryName))
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category name is required.", 400);

                if (string.IsNullOrWhiteSpace(category.CategoryCode))
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category code is required.", 400);

                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                if (categories.Any(c =>
                    c.CategoryCode == category.CategoryCode))
                {
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category code already exists.", 409);
                }

                ProductCategory? parentCategory = null;

                if (category.ParentId.HasValue)
                {
                    parentCategory = await _categoryRepository
                        .GetByIdAsync(category.ParentId.Value);

                    if (parentCategory == null)
                        return ResponseDto<ProductCategoryDto>.NotFound("Parent category not found.");
                            
                }

                var newCategory = new ProductCategory
                {
                    CategoryCode = category.CategoryCode.Trim(),
                    CategoryName = category.CategoryName.Trim(),
                    ParentId = category.ParentId
                };

                await _categoryRepository.AddAsync(newCategory);
                await _unitOfWork.SaveChangesAsync();

                var result = new ProductCategoryDto
                {
                    Id = newCategory.Id,
                    CategoryCode = newCategory.CategoryCode,
                    CategoryName = newCategory.CategoryName,
                    ParentId = newCategory.ParentId,
                    ParentCategoryName = parentCategory?.CategoryName
                };

                return ResponseDto<ProductCategoryDto>.Success(result, "Category created successfully.");
                    
            }
            catch (Exception ex)
            {
                return ResponseDto<ProductCategoryDto>.Error("An unexpected error occurred.", 500);
            }
        }

        public async Task<ResponseDto<ProductCategoryDto>> UpdateProductCategory(
            UpdateProductCategoryDto category)
        {
            try
            {
                if (category == null)
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category data is required.", 400);

                if (category.Id <= 0)
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Invalid category ID.", 400);

                if (string.IsNullOrWhiteSpace(category.CategoryName) ||
                    string.IsNullOrWhiteSpace(category.CategoryCode))
                {
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category name and code are required.", 400);
                }

                var existingCategory = await _categoryRepository
                    .GetByIdAsync(category.Id);

                if (existingCategory == null)
                    return ResponseDto<ProductCategoryDto>.NotFound(
                        "Category not found.");

                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                if (categories.Any(c =>
                    c.Id != category.Id &&
                    c.CategoryCode == category.CategoryCode))
                {
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Category code already exists.", 409);
                }

                ProductCategory? parentCategory = null;

                if (category.ParentId.HasValue)
                {
                    if (category.ParentId.Value == category.Id)
                        return ResponseDto<ProductCategoryDto>.Error(
                            "A category cannot be its own parent.", 400);

                    parentCategory = await _categoryRepository
                        .GetByIdAsync(category.ParentId.Value);

                    if (parentCategory == null)
                        return ResponseDto<ProductCategoryDto>.NotFound(
                            "Parent category not found.");

                    // Prevent circular parent-child relationships
                    var currentParentId = parentCategory.ParentId;

                    while (currentParentId.HasValue)
                    {
                        if (currentParentId.Value == category.Id)
                        {
                            return ResponseDto<ProductCategoryDto>.Error(
                                "Circular category relationship is not allowed.", 400);
                        }

                        var ancestor = categories.FirstOrDefault(
                            c => c.Id == currentParentId.Value);

                        currentParentId = ancestor?.ParentId;
                    }
                }

                existingCategory.CategoryCode = category.CategoryCode.Trim();
                existingCategory.CategoryName = category.CategoryName.Trim();
                existingCategory.ParentId = category.ParentId;

                _categoryRepository.Update(existingCategory);

                await _unitOfWork.SaveChangesAsync();

                var result = new ProductCategoryDto
                {
                    Id = existingCategory.Id,
                    CategoryCode = existingCategory.CategoryCode,
                    CategoryName = existingCategory.CategoryName,
                    ParentId = existingCategory.ParentId,
                    ParentCategoryName = parentCategory?.CategoryName
                };

                return ResponseDto<ProductCategoryDto>.Success(
                    result, "Category updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product category.");

                return ResponseDto<ProductCategoryDto>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        public async Task<ResponseDto<bool>> DeleteProductCategory(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return ResponseDto<bool>.Error(
                        "Invalid category ID.", 400);

                var category = await _categoryRepository.GetByIdAsync(categoryId);

                if (category == null)
                    return ResponseDto<bool>.NotFound(
                        "Category not found.");

                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                if (categories.Any(c => c.ParentId == categoryId))
                {
                    return ResponseDto<bool>.Error(
                        "Cannot delete a category that has subcategories.", 409);
                }

                var products = (await _productRepository.GetAllAsync()).ToList();

                if (products.Any(p => p.CategoryId == categoryId))
                {
                    return ResponseDto<bool>.Error(
                        "Cannot delete a category that has products.", 409);
                }

                _categoryRepository.Delete(category);

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true, "Category deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product category.");

                return ResponseDto<bool>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        public async Task<ResponseDto<IEnumerable<ProductCategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                if (!categories.Any())
                    return ResponseDto<IEnumerable<ProductCategoryDto>>.NotFound("No categories found.");
                        
                var result = BuildCategoryTree(categories, null);
                return ResponseDto<IEnumerable<ProductCategoryDto>>.Success(result, "Categories retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<ProductCategoryDto>>.Error("An unexpected error occurred.", 500);
            }
        }

        public async Task<ResponseDto<ProductCategoryDto>> GetCategoryById(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return ResponseDto<ProductCategoryDto>.Error(
                        "Invalid category ID.", 400);

                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                var category = categories.FirstOrDefault(c => c.Id == categoryId);

                if (category == null)
                    return ResponseDto<ProductCategoryDto>.NotFound("Category not found.");
                        

                var result = BuildCategoryTree(categories, categoryId).First();

                return ResponseDto<ProductCategoryDto>.Success(result, "Category retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<ProductCategoryDto>.Error("An unexpected error occurred.", 500);
            }
        }

        private List<ProductCategoryDto> BuildCategoryTree(
            List<ProductCategory> categories,
            int? parentId)
        {
            return categories
                .Where(c => c.ParentId == parentId)
                .Select(c => new ProductCategoryDto
                {
                    Id = c.Id,
                    CategoryCode = c.CategoryCode,
                    CategoryName = c.CategoryName,
                    ParentId = c.ParentId,
                    ParentCategoryName = categories
                        .FirstOrDefault(p => p.Id == c.ParentId)?.CategoryName,

                    SubCategories = BuildCategoryTree(categories, c.Id)
                })
                .ToList();
        }

        public async Task<ResponseDto<IEnumerable<ProductCategoryDto>>> GetCategoriesByParentId(
                 int parentId)
        {
            try
            {
                if (parentId <= 0)
                {
                    return ResponseDto<IEnumerable<ProductCategoryDto>>.Error(
                        "Invalid parent category ID.", 400);
                }

                var parentCategory = await _categoryRepository.GetByIdAsync(parentId);

                if (parentCategory == null)
                {
                    return ResponseDto<IEnumerable<ProductCategoryDto>>.NotFound(
                        "Parent category not found.");
                }

                var categories = (await _categoryRepository.GetAllAsync()).ToList();

                var result = BuildCategoryTree(categories, parentId);

                if (!result.Any())
                {
                    return ResponseDto<IEnumerable<ProductCategoryDto>>.NotFound(
                        "No subcategories found.");
                }

                return ResponseDto<IEnumerable<ProductCategoryDto>>.Success(
                    result, "Subcategories retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving subcategories for parent category {ParentId}.",
                    parentId);

                return ResponseDto<IEnumerable<ProductCategoryDto>>.Error(
                    "An unexpected error occurred.", 500);
            }
        }


    }

}
