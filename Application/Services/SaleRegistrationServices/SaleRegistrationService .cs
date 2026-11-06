using Application.Interfaces.SalesRegistration;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.SaleRegistration;
using Domain.Entities.Products;
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

        public SaleRegistrationService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _saleRepository = _unitOfWork.Repository<SaleRegistration>();
            _productRepository = _unitOfWork.Repository<Product>();
            _channelRepository = _unitOfWork.Repository<SalesChannel>();
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

        public async Task<ResponseDto<SaleRegistrationDto>>
            CreateSaleRegistration(CreateSaleRegistrationDto dto)
        {
            try
            {
                if (dto == null)
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

                var product = await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "محصول موردنظر یافت نشد.");

                var channel = await _channelRepository.GetByIdAsync(dto.ChannelId);

                if (channel == null)
                    return ResponseDto<SaleRegistrationDto>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

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

                await _saleRepository.AddAsync(sale);
                await _unitOfWork.SaveChangesAsync();

                sale.Product = product;
                sale.SalesChannel = channel;

                return ResponseDto<SaleRegistrationDto>.Success(
                    MapToDto(sale),
                    "ثبت اطلاعات فروش با موفقیت انجام شد.");
            }
            catch (Exception ex)
            {


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
    }
}
