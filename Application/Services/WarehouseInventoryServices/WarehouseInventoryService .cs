using Application.Interfaces.IWarehouseInventory;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.WarehouseInventory;
using Domain.Entities.Inventories;
using Domain.Entities.Products;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.WarehouseInventoryServices
{
    public class WarehouseInventoryService : IWarehouseInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<WarehouseInventory> _repository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly ILogger<WarehouseInventoryService> _logger;

        public WarehouseInventoryService(
            IUnitOfWork unitOfWork,
            ILogger<WarehouseInventoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository = _unitOfWork.Repository<WarehouseInventory>();
            _productRepository = _unitOfWork.Repository<Product>();
        }

        public async Task<ResponseDto<List<WarehouseInventoryDto>>> GetAllInventories()
        {
            try
            {
                var inventories = await _repository.GetAllAsync();
                var products = await _productRepository.GetAllAsync();

                var productDictionary = products.ToDictionary(x => x.Id);

                var result = inventories.Select(inventory =>
                {
                    productDictionary.TryGetValue(inventory.ProductId, out var product);

                    return new WarehouseInventoryDto
                    {
                        Id = inventory.Id,
                        CurrentInventory = inventory.CurrentInventory,
                        ProductId = inventory.ProductId,
                        ProductName = product?.ProductName ?? string.Empty
                    };
                }).ToList();

                return ResponseDto<List<WarehouseInventoryDto>>.Success(
                    result,
                    "موجودی انبار با موفقیت دریافت شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting all warehouse inventories");

                return ResponseDto<List<WarehouseInventoryDto>>.Error(
                    "خطا در دریافت موجودی انبار",
                    500);
            }
        }

        public async Task<ResponseDto<WarehouseInventoryDto>> GetInventoryById(int id)
        {
            try
            {
                var inventory = await _repository.GetByIdAsync(id);

                if (inventory == null)
                {
                    return ResponseDto<WarehouseInventoryDto>.NotFound(
                        "رکورد موجودی انبار یافت نشد");
                }

                var product = await _productRepository.GetByIdAsync(inventory.ProductId);

                var result = new WarehouseInventoryDto
                {
                    Id = inventory.Id,
                    CurrentInventory = inventory.CurrentInventory,
                    ProductId = inventory.ProductId,
                    ProductName = product?.ProductName ?? string.Empty
                };

                return ResponseDto<WarehouseInventoryDto>.Success(
                    result,
                    "موجودی محصول با موفقیت دریافت شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting inventory by ID: {Id}", id);

                return ResponseDto<WarehouseInventoryDto>.Error(
                    "خطا در دریافت موجودی محصول",
                    500);
            }
        }

        public async Task<ResponseDto<WarehouseInventoryDto>> GetInventoryByProductId(int productId)
        {
            try
            {
                var inventories = await _repository.GetAllAsync();

                var inventory = inventories.FirstOrDefault(
                    x => x.ProductId == productId);

                if (inventory == null)
                {
                    return ResponseDto<WarehouseInventoryDto>.NotFound(
                        "برای این محصول رکورد موجودی ثبت نشده است");
                }

                var product = await _productRepository.GetByIdAsync(productId);

                var result = new WarehouseInventoryDto
                {
                    Id = inventory.Id,
                    CurrentInventory = inventory.CurrentInventory,
                    ProductId = inventory.ProductId,
                    ProductName = product?.ProductName ?? string.Empty
                };

                return ResponseDto<WarehouseInventoryDto>.Success(
                    result,
                    "موجودی محصول با موفقیت دریافت شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while getting inventory for ProductId: {ProductId}",
                    productId);

                return ResponseDto<WarehouseInventoryDto>.Error(
                    "خطا در دریافت موجودی محصول",
                    500);
            }
        }

        public async Task<bool> ReduceInventoryAsync(int productId, int quantity)
        {
            var inventory = await _repository
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (inventory == null || inventory.CurrentInventory < quantity)
                return false;

            inventory.CurrentInventory -= quantity;

            return true;
        }
    }
}
