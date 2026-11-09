using Application.Interfaces.IWarehouseInventory;
using Application.Interfaces.ProductAllocations;
using Application.Interfaces.SalesRegistration;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.SaleRegistration;
using Domain.Entities.Inventories;
using Domain.Entities.Products;
using Domain.Entities.SalesChannels;
using Domain.Entities.Salse;
using Domain.Entities.SellingChannels;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.SaleRegistrationServices
{
    public class SaleRegistrationService : ISaleRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<SaleRegistration> _saleRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<SalesChannel> _channelRepository;
        private readonly IGenericRepository<WarehouseInventory> _warehouseInventory;
        private readonly IGenericRepository<ProductChannelAllocation> _productChannelAllocation;
        private readonly IWarehouseInventoryService _wareHouseService;
        private readonly IProductChannelAllocationService _allocationService;


        public SaleRegistrationService(
            IUnitOfWork unitOfWork, IWarehouseInventoryService wareHouseService, IProductChannelAllocationService allocationService)
        {
            _unitOfWork = unitOfWork;
            _saleRepository = _unitOfWork.Repository<SaleRegistration>();
            _productRepository = _unitOfWork.Repository<Product>();
            _channelRepository = _unitOfWork.Repository<SalesChannel>();
            _warehouseInventory = _unitOfWork.Repository<WarehouseInventory>();
            _productChannelAllocation = _unitOfWork.Repository<ProductChannelAllocation>();
            _wareHouseService = wareHouseService;
            _allocationService = allocationService;
        }

        public async Task<ResponseDto<SaleRegistrationDto>>
            CreateSaleRegistration(CreateSaleRegistrationDto dto)
        {
            bool transactionStarted = false;

            try
            {
                // 1. اعتبارسنجی اطلاعات ورودی
                var validation = ValidateSaleRequest(dto);

                if (validation != null)
                    return ResponseDto<SaleRegistrationDto>.Error(validation);

                // 2. دریافت محصول
                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "محصول موردنظر یافت نشد.");

                // 3. دریافت کانال فروش
                var channel = await _channelRepository.GetByIdAsync(dto.ChannelId);

                if (channel == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                // 4. بررسی اولیه موجودی
                var inventoryValidation =
                    await ValidateInventoryAsync(dto, product.ProductName);

                if (inventoryValidation != null)
                    return ResponseDto<SaleRegistrationDto>.Error(inventoryValidation);

                // 5. شروع تراکنش
                await _unitOfWork.BeginTransactionAsync();
                transactionStarted = true;

                // 6. کاهش موجودی انبار
                var warehouseResult = await _wareHouseService
                    .ReduceInventoryAsync(dto.ProductId, dto.ProductQuantity);

                if (!warehouseResult)
                    throw new InvalidOperationException(
                        "کاهش موجودی انبار انجام نشد.");

                // 7. کاهش تخصیص کانال
                var allocationResult = await _allocationService
                    .ReduceChannelAllocationAsync(
                        dto.ProductId,
                        dto.ChannelId,
                        dto.ProductQuantity);

                if (!allocationResult)
                    throw new InvalidOperationException(
                        "کاهش موجودی تخصیص کانال انجام نشد.");

                // 8. ساخت و ثبت فروش
                var sale = CreateSaleEntity(dto);

                await _saleRepository.AddAsync(sale);

                // 9. ذخیره تمام تغییرات
                await _unitOfWork.SaveChangesAsync();

                // 10. تأیید تراکنش
                await _unitOfWork.CommitTransactionAsync();
                transactionStarted = false;

                sale.Product = product;
                sale.SalesChannel = channel;

                return ResponseDto<SaleRegistrationDto>.Success(
                    MapToDto(sale),
                    "ثبت اطلاعات فروش با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا، تمام تغییرات برگردانده می‌شوند.
                if (transactionStarted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }

                return ResponseDto<SaleRegistrationDto>.Error(
                    "در ثبت اطلاعات فروش خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<SaleRegistrationDto>>
            UpdateSaleRegistration(UpdateSaleRegistrationDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0)
                    return ResponseDto<SaleRegistrationDto>.Error(
                        "اطلاعات ارسالی معتبر نیست.");

                var validation = ValidateFinancialData(
                    dto.ProductQuantity,
                    dto.SalePrice,
                    dto.BuyPrice,
                    dto.SendPrice,
                    dto.PackingPrice,
                    dto.CommissionRate);

                if (validation != null)
                    return ResponseDto<SaleRegistrationDto>.Error(validation);

                if (dto.ProductId <= 0 || dto.ChannelId <= 0)
                    return ResponseDto<SaleRegistrationDto>.Error(
                        "محصول یا کانال فروش معتبر نیست.");

                var sale = await _saleRepository.GetByIdAsync(dto.Id);

                if (sale == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "رکورد فروش موردنظر یافت نشد.");

                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "محصول موردنظر یافت نشد.");

                var channel = await _channelRepository.GetByIdAsync(dto.ChannelId);

                if (channel == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                sale.ProductQuantity = dto.ProductQuantity;
                sale.SalePrice = dto.SalePrice;
                sale.BuyPrice = dto.BuyPrice;
                sale.SendPrice = dto.SendPrice;
                sale.PackingPrice = dto.PackingPrice;
                sale.CommissionRate = dto.CommissionRate;
                sale.ProductId = dto.ProductId;
                sale.ChannelId = dto.ChannelId;

                sale.CalculateFinancials();

                _saleRepository.Update(sale);
                await _unitOfWork.SaveChangesAsync();

                sale.Product = product;
                sale.SalesChannel = channel;

                return ResponseDto<SaleRegistrationDto>.Success(
                    MapToDto(sale),
                    "اطلاعات فروش با موفقیت ویرایش شد.");
            }
            catch (Exception ex)
            {


                return ResponseDto<SaleRegistrationDto>.Error(
                    "در ویرایش اطلاعات فروش خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<bool>> DeleteSaleRegistration(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseDto<bool>.Error("شناسه معتبر نیست.");

                var sale = await _saleRepository.GetByIdAsync(id);

                if (sale == null)
                    return ResponseDto<bool>.NotFound(
                        "رکورد فروش موردنظر یافت نشد.");

                _saleRepository.Delete(sale);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true,
                    "رکورد فروش با موفقیت حذف شد.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Error(
                    "در حذف رکورد فروش خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<IEnumerable<SaleRegistrationDto>>>
            GetAllSaleRegistrations()
        {
            try
            {
                var sales = await _saleRepository.GetAllAsync();

                var result = sales
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Success(
                    result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Error(
                    "در دریافت اطلاعات فروش خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<SaleRegistrationDto>>
            GetSaleRegistrationById(int id)
        {
            try
            {
                if (id <= 0)
                    return ResponseDto<SaleRegistrationDto>.Error(
                        "شناسه معتبر نیست.");

                var sale = await _saleRepository.GetByIdAsync(id);

                if (sale == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "رکورد فروش یافت نشد.");

                return ResponseDto<SaleRegistrationDto>.Success(
                    MapToDto(sale));
            }
            catch (Exception ex)
            {
                return ResponseDto<SaleRegistrationDto>.Error(
                    "در دریافت اطلاعات فروش خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<IEnumerable<SaleRegistrationDto>>>
            GetByProductId(int productId)
        {
            try
            {
                if (productId <= 0)
                    return ResponseDto<IEnumerable<SaleRegistrationDto>>.Error(
                        "شناسه محصول معتبر نیست.");

                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                    return ResponseDto<IEnumerable<SaleRegistrationDto>>.NotFound(
                        "محصول موردنظر یافت نشد.");

                var sales = await _saleRepository.GetAllAsync();

                var result = sales
                    .Where(s => s.ProductId == productId)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Success(
                    result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Error(
                    "در دریافت سوابق فروش محصول خطایی رخ داد.", 500);
            }
        }

        public async Task<ResponseDto<IEnumerable<SaleRegistrationDto>>>
            GetByChannelId(int channelId)
        {
            try
            {
                if (channelId <= 0)
                    return ResponseDto<IEnumerable<SaleRegistrationDto>>.Error(
                        "شناسه کانال فروش معتبر نیست.");

                var channel = await _channelRepository.GetByIdAsync(channelId);

                if (channel == null)
                    return ResponseDto<IEnumerable<SaleRegistrationDto>>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                var sales = await _saleRepository.GetAllAsync();

                var result = sales
                    .Where(s => s.ChannelId == channelId)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Success(
                    result);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<SaleRegistrationDto>>.Error(
                    "در دریافت سوابق کانال فروش خطایی رخ داد.", 500);
            }
        }


        #region Privatemethods and Helpers
        private async Task<string?> ValidateInventoryAsync(
         CreateSaleRegistrationDto dto,
         string productName)
        {
            // بررسی موجودی انبار
            bool isAvailable = await _warehouseInventory.AnyAsync(x =>
                x.ProductId == dto.ProductId &&
                x.CurrentInventory >= dto.ProductQuantity
            );

            if (!isAvailable)
                return $"موجودی محصول {productName} در انبار کافی نیست.";

            // بررسی سهمیه اختصاص داده شده به کانال فروش
            bool isAllocated = await _productChannelAllocation.AnyAsync(x =>
                x.ProductId == dto.ProductId &&
                x.ChannelId == dto.ChannelId &&
                (x.AllocatedQuantity - x.SoldQuantity) >= dto.ProductQuantity
            );

            if (!isAllocated)
                return $"این تعداد محصول {productName} به کانال فروش تخصیص پیدا نکرده است.";

            return null;
        }

        private SaleRegistration CreateSaleEntity(
              CreateSaleRegistrationDto dto)
        {
            var sale = new SaleRegistration
            {
                ProductQuantity = dto.ProductQuantity,
                SalePrice = dto.SalePrice,
                BuyPrice = dto.BuyPrice,
                SendPrice = dto.SendPrice,
                PackingPrice = dto.PackingPrice,
                CommissionRate = dto.CommissionRate,
                ProductId = dto.ProductId,
                ChannelId = dto.ChannelId
            };

            sale.CalculateFinancials();

            return sale;
        }

        private static string? ValidateFinancialData(
           int quantity,
           decimal salePrice,
           decimal buyPrice,
           decimal sendPrice,
           decimal packingPrice,
           decimal commissionRate)
        {
            if (quantity <= 0)
                return "تعداد محصول باید بیشتر از صفر باشد.";

            if (salePrice < 0 || buyPrice < 0 ||
                sendPrice < 0 || packingPrice < 0)
                return "مقادیر قیمت و هزینه نمی‌توانند منفی باشند.";

            if (commissionRate < 0 || commissionRate > 100)
                return "درصد کمیسیون باید بین صفر تا صد باشد.";

            return null;
        }

        private static SaleRegistrationDto MapToDto(
          SaleRegistration sale)
        {
            return new SaleRegistrationDto
            {
                Id = sale.Id,
                ProductQuantity = sale.ProductQuantity,
                SalePrice = sale.SalePrice,
                BuyPrice = sale.BuyPrice,
                SendPrice = sale.SendPrice,
                PackingPrice = sale.PackingPrice,
                CommissionRate = sale.CommissionRate,

                TotalSaleAmount = sale.TotalSaleAmount,
                PurchaseNetCost = sale.PurchaseNetCost,
                CommissionCost = sale.CommissionCost,
                NetProfit = sale.NetProfit,
                NetProfitPercent = sale.NetProfitPercent,

                ProductId = sale.ProductId,
                ProductName = sale.Product?.ProductName ?? string.Empty,

                ChannelId = sale.ChannelId,
                ChannelName = sale.SalesChannel?.Name ?? string.Empty
            };
        }

        private string? ValidateSaleRequest(CreateSaleRegistrationDto dto)
        {
            if (dto == null)
                return "اطلاعات ارسالی معتبر نیست.";

            var validation = ValidateFinancialData(
                dto.ProductQuantity,
                dto.SalePrice,
                dto.BuyPrice,
                dto.SendPrice,
                dto.PackingPrice,
                dto.CommissionRate);

            if (validation != null)
                return validation;

            if (dto.ProductId <= 0 || dto.ChannelId <= 0)
                return "محصول یا کانال فروش معتبر نیست.";

            return null;
        }
        #endregion
    }
}
