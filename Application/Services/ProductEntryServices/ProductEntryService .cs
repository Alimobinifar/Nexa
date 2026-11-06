using Application.Interfaces.ProductEntries;
using Contracts.Cmmon.Dtos.ProductEntries;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Domain.Entities.Products;
using Domain.Entities.Suppliers;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.ProductEntryServices
{
    public class ProductEntryService : IProductEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ProductEntry> _repository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Supplier> _supplierRepository;
        private readonly ILogger<ProductEntryService> _logger;

        public ProductEntryService(
            IUnitOfWork unitOfWork,
            ILogger<ProductEntryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository = _unitOfWork.Repository<ProductEntry>();
            _productRepository = _unitOfWork.Repository<Product>();
            _supplierRepository = _unitOfWork.Repository<Supplier>();
        }

        // Create
        public async Task<ResponseDto<ProductEntryDto>> CreateProductEntry(
            CreateProductEntryDto dto)
        {
            try
            {
                if (dto == null)
                    return ResponseDto<ProductEntryDto>.Error("اطلاعات ورودی نامعتبر است");

                if (dto.ProductId <= 0)
                    return ResponseDto<ProductEntryDto>.Error("محصول انتخاب‌شده معتبر نیست");

                if (dto.Quantity <= 0)
                    return ResponseDto<ProductEntryDto>.Error("تعداد کالا باید بیشتر از صفر باشد");

                if (dto.UnitPurchasePrice < 0)
                    return ResponseDto<ProductEntryDto>.Error("قیمت خرید نمی‌تواند منفی باشد");

                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<ProductEntryDto>.NotFound("محصول موردنظر یافت نشد");

                Supplier? supplier = null;

                if (dto.SupplierId.HasValue)
                {
                    supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId.Value);

                    if (supplier == null)
                        return ResponseDto<ProductEntryDto>.NotFound("تأمین‌کننده موردنظر یافت نشد");
                }

                var entry = new ProductEntry
                {
                    EntryDate = dto.EntryDate == default
                        ? DateTime.UtcNow
                        : dto.EntryDate,

                    Quantity = dto.Quantity,
                    UnitPurchasePrice = dto.UnitPurchasePrice,
                    Notes = dto.Notes,
                    ProductId = dto.ProductId,
                    SupplierId = dto.SupplierId
                };

                await _repository.AddAsync(entry);
                await _unitOfWork.SaveChangesAsync();

                var result = MapToDto(
                    entry,
                    product.ProductName,
                    supplier?.Name);

                return ResponseDto<ProductEntryDto>.Success(
                    result,
                    "ورود کالا با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product entry");

                return ResponseDto<ProductEntryDto>.Error(
                    "خطا در ثبت ورود کالا",
                    500);
            }
        }

        // Update
        public async Task<ResponseDto<ProductEntryDto>> UpdateProductEntry(
            UpdateProductEntryDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return ResponseDto<ProductEntryDto>.Error("اطلاعات ورودی نامعتبر است");

                if (dto.ProductId <= 0)
                    return ResponseDto<ProductEntryDto>.Error("محصول انتخاب‌شده معتبر نیست");

                if (dto.Quantity <= 0)
                    return ResponseDto<ProductEntryDto>.Error("تعداد کالا باید بیشتر از صفر باشد");

                if (dto.UnitPurchasePrice < 0)
                    return ResponseDto<ProductEntryDto>.Error("قیمت خرید نمی‌تواند منفی باشد");

                var entry = await _repository.GetByIdAsync(dto.Id);

                if (entry == null)
                    return ResponseDto<ProductEntryDto>.NotFound("رکورد ورود کالا یافت نشد");

                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<ProductEntryDto>.NotFound("محصول موردنظر یافت نشد");

                Supplier? supplier = null;

                if (dto.SupplierId.HasValue)
                {
                    supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId.Value);

                    if (supplier == null)
                        return ResponseDto<ProductEntryDto>.NotFound("تأمین‌کننده موردنظر یافت نشد");
                }

                entry.EntryDate = dto.EntryDate == default
                    ? entry.EntryDate
                    : dto.EntryDate;

                entry.Quantity = dto.Quantity;
                entry.UnitPurchasePrice = dto.UnitPurchasePrice;
                entry.Notes = dto.Notes;
                entry.ProductId = dto.ProductId;
                entry.SupplierId = dto.SupplierId;

                _repository.Update(entry);
                await _unitOfWork.SaveChangesAsync();

                var result = MapToDto(
                    entry,
                    product.ProductName,
                    supplier?.Name);

                return ResponseDto<ProductEntryDto>.Success(
                    result,
                    "اطلاعات ورود کالا با موفقیت ویرایش شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product entry {Id}", dto?.Id);

                return ResponseDto<ProductEntryDto>.Error(
                    "خطا در ویرایش ورود کالا",
                    500);
            }
        }

        // Delete
        public async Task<ResponseDto<bool>> DeleteProductEntry(int id)
        {
            try
            {
                var entry = await _repository.GetByIdAsync(id);

                if (entry == null)
                    return ResponseDto<bool>.NotFound("رکورد ورود کالا یافت نشد");

                _repository.Delete(entry);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true,
                    "رکورد ورود کالا با موفقیت حذف شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product entry {Id}", id);

                return ResponseDto<bool>.Error(
                    "خطا در حذف رکورد ورود کالا",
                    500);
            }
        }

        // Get All
        public async Task<ResponseDto<List<ProductEntryDto>>> GetAllProductEntries()
        {
            try
            {
                var entries = (await _repository.GetAllAsync()).ToList();

                var products = (await _productRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.ProductName);

                var suppliers = (await _supplierRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.Name);

                var result = entries.Select(entry =>
                    MapToDto(
                        entry,
                        products.GetValueOrDefault(entry.ProductId, "نامشخص"),
                        entry.SupplierId.HasValue
                            ? suppliers.GetValueOrDefault(entry.SupplierId.Value)
                            : null))
                    .ToList();

                return ResponseDto<List<ProductEntryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product entries");

                return ResponseDto<List<ProductEntryDto>>.Error(
                    "خطا در دریافت سوابق ورود کالا",
                    500);
            }
        }

        // Get By Id
        public async Task<ResponseDto<ProductEntryDto>> GetProductEntryById(int id)
        {
            try
            {
                var entry = await _repository.GetByIdAsync(id);

                if (entry == null)
                    return ResponseDto<ProductEntryDto>.NotFound("رکورد ورود کالا یافت نشد");

                var product = await _productRepository.GetByIdAsync(entry.ProductId);

                Supplier? supplier = null;

                if (entry.SupplierId.HasValue)
                    supplier = await _supplierRepository.GetByIdAsync(entry.SupplierId.Value);

                var result = MapToDto(
                    entry,
                    product?.ProductName ?? "نامشخص",
                    supplier?.Name);

                return ResponseDto<ProductEntryDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product entry {Id}", id);

                return ResponseDto<ProductEntryDto>.Error(
                    "خطا در دریافت اطلاعات ورود کالا",
                    500);
            }
        }

        // Get By Product
        public async Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesByProductId(
            int productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                    return ResponseDto<List<ProductEntryDto>>.NotFound("محصول موردنظر یافت نشد");

                var entries = (await _repository.GetAllAsync())
                    .Where(x => x.ProductId == productId)
                    .ToList();

                var suppliers = (await _supplierRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.Name);

                var result = entries.Select(entry =>
                    MapToDto(
                        entry,
                        product.ProductName,
                        entry.SupplierId.HasValue
                            ? suppliers.GetValueOrDefault(entry.SupplierId.Value)
                            : null))
                    .ToList();

                return ResponseDto<List<ProductEntryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entries for product {ProductId}", productId);

                return ResponseDto<List<ProductEntryDto>>.Error(
                    "خطا در دریافت سوابق محصول",
                    500);
            }
        }

        // Get By Supplier
        public async Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesBySupplierId(
            int supplierId)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(supplierId);

                if (supplier == null)
                    return ResponseDto<List<ProductEntryDto>>.NotFound("تأمین‌کننده موردنظر یافت نشد");

                var entries = (await _repository.GetAllAsync())
                    .Where(x => x.SupplierId == supplierId)
                    .ToList();

                var products = (await _productRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.ProductName);

                var result = entries.Select(entry =>
                    MapToDto(
                        entry,
                        products.GetValueOrDefault(entry.ProductId, "نامشخص"),
                        supplier.Name))
                    .ToList();

                return ResponseDto<List<ProductEntryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting entries for supplier {SupplierId}", supplierId);

                return ResponseDto<List<ProductEntryDto>>.Error(
                    "خطا در دریافت سوابق تأمین‌کننده",
                    500);
            }
        }

        // Get By Date Range
        public async Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesByDateRange(
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return ResponseDto<List<ProductEntryDto>>.Error(
                        "تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد");

                var entries = (await _repository.GetAllAsync())
                    .Where(x => x.EntryDate >= startDate && x.EntryDate <= endDate)
                    .ToList();

                var products = (await _productRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.ProductName);

                var suppliers = (await _supplierRepository.GetAllAsync())
                    .ToDictionary(x => x.Id, x => x.Name);

                var result = entries.Select(entry =>
                    MapToDto(
                        entry,
                        products.GetValueOrDefault(entry.ProductId, "نامشخص"),
                        entry.SupplierId.HasValue
                            ? suppliers.GetValueOrDefault(entry.SupplierId.Value)
                            : null))
                    .ToList();

                return ResponseDto<List<ProductEntryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product entries by date range");

                return ResponseDto<List<ProductEntryDto>>.Error(
                    "خطا در دریافت سوابق بر اساس تاریخ",
                    500);
            }
        }

        // Mapper
        private static ProductEntryDto MapToDto(
            ProductEntry entry,
            string productName,
            string? supplierName)
        {
            return new ProductEntryDto
            {
                Id = entry.Id,
                EntryDate = entry.EntryDate,
                Quantity = entry.Quantity,
                UnitPurchasePrice = entry.UnitPurchasePrice,
                TotalValue = entry.TotalValue,
                Notes = entry.Notes,
                ProductId = entry.ProductId,
                ProductName = productName,
                SupplierId = entry.SupplierId,
                SupplierName = supplierName
            };
        }
    }
}
