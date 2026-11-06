using Application.Interfaces.Products;
using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Domain.Entities.Product;
using Domain.Entities.Products;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Product> _repository;
        private readonly IGenericRepository<ProductCategory> _categoryRepository;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repository = _unitOfWork.Repository<Product>();
            _categoryRepository = _unitOfWork.Repository<ProductCategory>();
        }

        public async Task<ResponseDto<CreateProductDto>> CreateProduct(CreateProductDto product)
        {
            try
            {
                if (product == null)
                {
                    return ResponseDto<CreateProductDto>.Error(
                        "Product data is required.", 400);
                }

                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    return ResponseDto<CreateProductDto>.Error(
                        "Product name is required.", 400);
                }

                var category = await _categoryRepository
                    .GetByIdAsync(product.CategoryId);

                if (category == null)
                {
                    return ResponseDto<CreateProductDto>.NotFound(
                        "Category not found.");
                }

                var newProduct = new Product
                {
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    CategoryId = product.CategoryId
                };

                await _repository.AddAsync(newProduct);

                await _unitOfWork.SaveChangesAsync();

                var result = new CreateProductDto
                {
                    ProductName = newProduct.ProductName,
                    ProductDescription = newProduct.ProductDescription,
                    CategoryId = newProduct.CategoryId
                };

                return ResponseDto<CreateProductDto>.Success(
                    result, "Product created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<CreateProductDto>.Error(
                    "An unexpected error occurred.", 500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResponseDto<UpdateProductDto>> UpdateProduct(UpdateProductDto product)
        {
            try
            {
                if (product == null)
                {
                    return ResponseDto<UpdateProductDto>.Error(
                        "Product data is required.", 400);
                }

                if (product.ProductId <= 0)
                {
                    return ResponseDto<UpdateProductDto>.Error(
                        "Invalid ProductId.", 400);
                }

                var existingProduct = await _repository
                    .GetByIdAsync(product.ProductId);

                if (existingProduct == null)
                {
                    return ResponseDto<UpdateProductDto>.NotFound(
                        "Product not found.");
                }

                var category = await _categoryRepository
                    .GetByIdAsync(product.CategoryId);

                if (category == null)
                {
                    return ResponseDto<UpdateProductDto>.NotFound(
                        "Category not found.");
                }

                existingProduct.ProductName = product.ProductName;
                existingProduct.ProductDescription = product.ProductDescription;
                existingProduct.CategoryId = product.CategoryId;

                _repository.Update(existingProduct);

                await _unitOfWork.SaveChangesAsync();

                var result = new UpdateProductDto
                {
                    ProductId = existingProduct.Id,
                    ProductName = existingProduct.ProductName,
                    ProductDescription = existingProduct.ProductDescription,
                    CategoryId = existingProduct.CategoryId
                };

                return ResponseDto<UpdateProductDto>.Success(
                    result, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<UpdateProductDto>.Error(
                    "An unexpected error occurred.", 500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResponseDto<bool>> DeleteProduct(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return ResponseDto<bool>.Error(
                        "ProductId is required.", 400);
                }

                var product = await _repository.GetByIdAsync(productId);

                if (product == null)
                {
                    return ResponseDto<bool>.NotFound(
                        "Product not found.");
                }

                _repository.Delete(product);

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Error(
                    "An unexpected error occurred.", 500,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ResponseDto<IEnumerable<ProductDto>>> ProductsList()
        {
            try
            {
                var products = await _repository.GetAllAsync();

                if (products == null || !products.Any())
                {
                    return ResponseDto<IEnumerable<ProductDto>>.NotFound(
                        "No products found.");
                }

                var result = products.Select(p => new ProductDto
                {
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null
                        ? p.Category.CategoryName
                        : "",
                    ProductDescription = p.ProductDescription
                }).ToList();

                return ResponseDto<IEnumerable<ProductDto>>.Success(
                    result, "Products retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<ProductDto>>.Error(
                    "An unexpected error occurred.", 500,
                    new List<string> { ex.Message });
            }
        }

    }
}
