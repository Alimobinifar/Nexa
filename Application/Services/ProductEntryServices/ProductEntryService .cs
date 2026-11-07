using Application.Interfaces.ProductEntries;
using Contracts.Cmmon.Dtos.ProductEntries;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Domain.Entities.Inventories;
using Domain.Entities.Products;
using Domain.Entities.Suppliers;
using Domain.Enums;
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
        private readonly IGenericRepository<WarehouseInventory> _inventoryRepository;
        private readonly IGenericRepository<InventoryTransaction> _inventoryTransactionRepository;
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
            _inventoryRepository = _unitOfWork.Repository<WarehouseInventory>();
            _inventoryTransactionRepository = _unitOfWork.Repository<InventoryTransaction>();
        }

        // Create
        public async Task<ResponseDto<ProductEntryDto>> CreateProductEntry(
            CreateProductEntryDto dto)
        {
            var transactionStarted = false;

            try
            {
                if (dto == null)
                    return ResponseDto<ProductEntryDto>.Error(
                        "اطلاعات ورودی نامعتبر است");

                if (dto.ProductId <= 0)
                    return ResponseDto<ProductEntryDto>.Error(
                        "محصول انتخاب‌شده معتبر نیست");

                if (dto.Quantity <= 0)
                    return ResponseDto<ProductEntryDto>.Error(
                        "تعداد کالا باید بیشتر از صفر باشد");

                if (dto.UnitPurchasePrice < 0)
                    return ResponseDto<ProductEntryDto>.Error(
                        "قیمت خرید نمی‌تواند منفی باشد");

                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<ProductEntryDto>.NotFound(
                        "محصول موردنظر یافت نشد");

                Supplier? supplier = null;

                if (dto.SupplierId.HasValue)
                {
                    supplier = await _supplierRepository.GetByIdAsync(
                        dto.SupplierId.Value);

                    if (supplier == null)
                        return ResponseDto<ProductEntryDto>.NotFound(
                            "تأمین‌کننده موردنظر یافت نشد");
                }

                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // 1. ثبت ورود کالا
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

                // 2. دریافت یا ایجاد موجودی محصول
                var inventory = (await _inventoryRepository.GetAllAsync())
                    .FirstOrDefault(x => x.ProductId == dto.ProductId);

                decimal previousInventory = inventory?.CurrentInventory ?? 0;

                if (inventory == null)
                {
                    inventory = new WarehouseInventory
                    {
                        ProductId = dto.ProductId,
                        CurrentInventory = dto.Quantity
                    };

                    await _inventoryRepository.AddAsync(inventory);
                }
                else
                {
                    inventory.CurrentInventory += dto.Quantity;

                    _inventoryRepository.Update(inventory);
                }

                // 3. ثبت گردش موجودی
                var inventoryTransaction = new InventoryTransaction
                {
                    ProductId = dto.ProductId,
                    TransactionType = InventoryTransactionType.PurchaseEntry,
                    Quantity = dto.Quantity,
                    TransactionDate = entry.EntryDate,
                    ReferenceType = nameof(ProductEntry),
                    ReferenceId = entry.Id,
                    Description = dto.Notes
                };

                await _inventoryTransactionRepository.AddAsync(
                    inventoryTransaction);

                // ثبت و Commit هر سه عملیات
                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                var result = MapToDto(
                    entry,
                    product.ProductName,
                    supplier?.Name);

                return ResponseDto<ProductEntryDto>.Success(
                    result,
                    "ورود کالا و افزایش موجودی انبار با موفقیت ثبت شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product entry");

                if (transactionStarted)
                {
                    try
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(
                            rollbackEx,
                            "Error rolling back product entry transaction");
                    }
                }

                return ResponseDto<ProductEntryDto>.Error(
                    "خطا در ثبت ورود کالا",
                    500);
            }
        }
        public async Task<ResponseDto<ProductEntryDto>> UpdateProductEntry(
            UpdateProductEntryDto dto)
        {
            bool transactionStarted = false;

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

                // ذخیره اطلاعات قبلی قبل از تغییر
                var oldProductId = entry.ProductId;
                var oldQuantity = entry.Quantity;

                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // حالت اول: محصول تغییر نکرده است
                if (oldProductId == dto.ProductId)
                {
                    var difference = dto.Quantity - oldQuantity;

                    if (difference != 0)
                    {
                        var success = await ApplyInventoryChangeAsync(
                            oldProductId,
                            difference,
                            entry.Id);

                        if (!success)
                        {
                            await _unitOfWork.RollbackTransactionAsync();
                            transactionStarted = false;

                            return ResponseDto<ProductEntryDto>.Error(
                                "موجودی انبار برای اصلاح این ورود کافی نیست");
                        }
                    }
                }
                else
                {
                    // حالت دوم: محصول تغییر کرده است

                    // برگشت مقدار ورود قبلی از محصول قبلی
                    var decreaseSuccess = await ApplyInventoryChangeAsync(
                        oldProductId,
                        -oldQuantity,
                        entry.Id);

                    if (!decreaseSuccess)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        transactionStarted = false;

                        return ResponseDto<ProductEntryDto>.Error(
                            "موجودی محصول قبلی برای اصلاح این ورود کافی نیست");
                    }

                    // افزودن مقدار ورود جدید به محصول جدید
                    await ApplyInventoryChangeAsync(
                        dto.ProductId,
                        dto.Quantity,
                        entry.Id);
                }

                // ویرایش اطلاعات ProductEntry
                entry.EntryDate = dto.EntryDate == default
                    ? entry.EntryDate
                    : dto.EntryDate;

                entry.Quantity = dto.Quantity;
                entry.UnitPurchasePrice = dto.UnitPurchasePrice;
                entry.Notes = dto.Notes;
                entry.ProductId = dto.ProductId;
                entry.SupplierId = dto.SupplierId;

                _repository.Update(entry);

                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

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
                if (transactionStarted)
                {
                    try
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "Error rolling back product entry update");
                    }
                }

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

        private async Task<bool> ApplyInventoryChangeAsync(
                int productId,
                decimal quantityChange,
                int entryId)
        {
            if (quantityChange == 0)
                return true;

            var inventories = await _inventoryRepository.GetAllAsync();

            var inventory = inventories.FirstOrDefault(
                x => x.ProductId == productId);

            // کاهش موجودی
            if (quantityChange < 0)
            {
                var decreaseAmount = Math.Abs(quantityChange);

                if (inventory == null ||
                    inventory.CurrentInventory < decreaseAmount)
                {
                    return false;
                }

                inventory.CurrentInventory -= decreaseAmount;
                _inventoryRepository.Update(inventory);
            }
            // افزایش موجودی
            else
            {
                if (inventory == null)
                {
                    inventory = new WarehouseInventory
                    {
                        ProductId = productId,
                        CurrentInventory = quantityChange
                    };

                    await _inventoryRepository.AddAsync(inventory);
                }
                else
                {
                    inventory.CurrentInventory += quantityChange;
                    _inventoryRepository.Update(inventory);
                }
            }

            // ثبت تراکنش انبار
            var inventoryTransaction = new InventoryTransaction
            {
                ProductId = productId,

                Quantity = Math.Abs(quantityChange),

                TransactionType = quantityChange > 0
                    ? InventoryTransactionType.AdjustmentIncrease
                    : InventoryTransactionType.AdjustmentDecrease,

                TransactionDate = DateTime.UtcNow,

                ReferenceType = "ProductEntryCorrection",

                ReferenceId = entryId,

                Description = "اصلاح موجودی ناشی از ویرایش ورود کالا"
            };

            await _inventoryTransactionRepository.AddAsync(inventoryTransaction);

            return true;
        }
    }
}
