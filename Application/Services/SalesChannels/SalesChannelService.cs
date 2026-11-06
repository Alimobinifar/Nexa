using Application.Interfaces.SaleChannels;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.SaleChannels;
using Domain.Entities.Salse;
using Domain.Entities.SellingChannels;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.SalesChannels
{
    public class SalesChannelService : ISalesChannelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<SalesChannel> _repository;
        private readonly IGenericRepository<SaleRegistration> _saleRegistrationRepository;
        private readonly ILogger<SalesChannelService> _logger;

        public SalesChannelService(
            IUnitOfWork unitOfWork,
            ILogger<SalesChannelService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository = _unitOfWork.Repository<SalesChannel>();
            _saleRegistrationRepository =
                _unitOfWork.Repository<SaleRegistration>();
        }

        #region Create

        public async Task<ResponseDto<SalesChannelDto>> CreateSalesChannel(
            CreateSalesChannelDto dto)
        {
            try
            {
                if (dto == null)
                    return ResponseDto<SalesChannelDto>.Error(
                        "اطلاعات ارسالی معتبر نیست.");

                var validationError = ValidateSalesChannel(
                    dto.Name,
                    dto.DefaultCommissionRate,
                    dto.FixedFee,
                    dto.DisplayOrder);

                if (validationError != null)
                    return ResponseDto<SalesChannelDto>.Error(validationError);

                var channels = await _repository.GetAllAsync();

                bool nameExists = channels.Any(x =>
                    x.Name.Trim().Equals(
                        dto.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    return ResponseDto<SalesChannelDto>.Error(
                        "کانالی با این نام قبلاً ثبت شده است.",
                        409);

                var channel = new SalesChannel
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    DefaultCommissionRate = dto.DefaultCommissionRate,
                    FixedFee = dto.FixedFee,
                    WebsiteUrl = dto.WebsiteUrl?.Trim(),
                    SupportPhone = dto.SupportPhone?.Trim(),
                    IsActive = dto.IsActive,
                    DisplayOrder = dto.DisplayOrder
                };

                await _repository.AddAsync(channel);
                await _unitOfWork.SaveChangesAsync();

                var result = MapToDto(channel);

                return new ResponseDto<SalesChannelDto>(
                    result,
                    "کانال فروش با موفقیت ایجاد شد.",
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sales channel.");

                return ResponseDto<SalesChannelDto>.Error(
                    "در ایجاد کانال فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Update

        public async Task<ResponseDto<SalesChannelDto>> UpdateSalesChannel(
            UpdateSalesChannelDto dto)
        {
            try
            {
                if (dto == null)
                    return ResponseDto<SalesChannelDto>.Error(
                        "اطلاعات ارسالی معتبر نیست.");

                var validationError = ValidateSalesChannel(
                    dto.Name,
                    dto.DefaultCommissionRate,
                    dto.FixedFee,
                    dto.DisplayOrder);

                if (validationError != null)
                    return ResponseDto<SalesChannelDto>.Error(validationError);

                var channel = await _repository.GetByIdAsync(dto.Id);

                if (channel == null)
                    return ResponseDto<SalesChannelDto>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                var channels = await _repository.GetAllAsync();

                bool nameExists = channels.Any(x =>
                    x.Id != dto.Id &&
                    x.Name.Trim().Equals(
                        dto.Name.Trim(),
                        StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                    return ResponseDto<SalesChannelDto>.Error(
                        "کانال دیگری با این نام وجود دارد.",
                        409);

                channel.Name = dto.Name.Trim();
                channel.Description = dto.Description?.Trim();
                channel.DefaultCommissionRate = dto.DefaultCommissionRate;
                channel.FixedFee = dto.FixedFee;
                channel.WebsiteUrl = dto.WebsiteUrl?.Trim();
                channel.SupportPhone = dto.SupportPhone?.Trim();
                channel.IsActive = dto.IsActive;
                channel.DisplayOrder = dto.DisplayOrder;

                _repository.Update(channel);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<SalesChannelDto>.Success(
                    MapToDto(channel),
                    "کانال فروش با موفقیت ویرایش شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating sales channel with ID {Id}.",
                    dto?.Id);

                return ResponseDto<SalesChannelDto>.Error(
                    "در ویرایش کانال فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Delete

        public async Task<ResponseDto<bool>> DeleteSalesChannel(int id)
        {
            try
            {
                var channel = await _repository.GetByIdAsync(id);

                if (channel == null)
                    return ResponseDto<bool>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                var saleRegistrations =
                    await _saleRegistrationRepository.GetAllAsync();

                bool hasSales = saleRegistrations.Any(
                    x => x.ChannelId == id);

                if (hasSales)
                    return ResponseDto<bool>.Error(
                        "این کانال دارای سوابق فروش است و امکان حذف آن وجود ندارد. آن را غیرفعال کنید.",
                        409);

                _repository.Delete(channel);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true,
                    "کانال فروش با موفقیت حذف شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting sales channel with ID {Id}.",
                    id);

                return ResponseDto<bool>.Error(
                    "در حذف کانال فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get All

        public async Task<ResponseDto<List<SalesChannelDto>>> GetAllSalesChannels()
        {
            try
            {
                var channels = await _repository.GetAllAsync();

                var result = channels
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<List<SalesChannelDto>>.Success(
                    result,
                    "لیست کانال‌های فروش دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sales channels.");

                return ResponseDto<List<SalesChannelDto>>.Error(
                    "در دریافت لیست کانال‌های فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get By Id

        public async Task<ResponseDto<SalesChannelDto>> GetSalesChannelById(
            int id)
        {
            try
            {
                var channel = await _repository.GetByIdAsync(id);

                if (channel == null)
                    return ResponseDto<SalesChannelDto>.NotFound(
                        "کانال فروش موردنظر یافت نشد.");

                return ResponseDto<SalesChannelDto>.Success(
                    MapToDto(channel),
                    "اطلاعات کانال فروش دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting sales channel with ID {Id}.",
                    id);

                return ResponseDto<SalesChannelDto>.Error(
                    "در دریافت اطلاعات کانال فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Search

        public async Task<ResponseDto<List<SalesChannelDto>>> SearchSalesChannels(
            string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ResponseDto<List<SalesChannelDto>>.Error(
                        "عبارت جستجو نمی‌تواند خالی باشد.");

                var channels = await _repository.GetAllAsync();

                var result = channels
                    .Where(x =>
                        x.Name.Contains(
                            searchTerm.Trim(),
                            StringComparison.OrdinalIgnoreCase) ||
                        (x.Description != null &&
                         x.Description.Contains(
                            searchTerm.Trim(),
                            StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<List<SalesChannelDto>>.Success(
                    result,
                    "نتایج جستجوی کانال‌های فروش دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error searching sales channels.");

                return ResponseDto<List<SalesChannelDto>>.Error(
                    "در جستجوی کانال‌های فروش خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Private Methods

        private static string? ValidateSalesChannel(
            string name,
            decimal commissionRate,
            decimal fixedFee,
            int displayOrder)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "نام کانال فروش الزامی است.";

            if (name.Trim().Length > 150)
                return "نام کانال فروش نمی‌تواند بیشتر از ۱۵۰ کاراکتر باشد.";

            if (commissionRate < 0 || commissionRate > 100)
                return "درصد کمیسیون باید بین صفر تا صد باشد.";

            if (fixedFee < 0)
                return "هزینه ثابت نمی‌تواند منفی باشد.";

            if (displayOrder < 0)
                return "ترتیب نمایش نمی‌تواند منفی باشد.";

            return null;
        }

        private static SalesChannelDto MapToDto(SalesChannel channel)
        {
            return new SalesChannelDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
                DefaultCommissionRate = channel.DefaultCommissionRate,
                FixedFee = channel.FixedFee,
                WebsiteUrl = channel.WebsiteUrl,
                SupportPhone = channel.SupportPhone,
                IsActive = channel.IsActive,
                DisplayOrder = channel.DisplayOrder
            };
        }

        #endregion
    }
}
