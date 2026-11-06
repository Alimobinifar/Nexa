using Application.Interfaces.ProductAllocations;
using Contracts.Cmmon.Dtos.ProductChannelAllocation;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Domain.Entities.Products;
using Domain.Entities.SalesChannels;
using Domain.Entities.SellingChannels;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.ProductChannelAllocationServices
{
    public class ProductChannelAllocationService
        : IProductChannelAllocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IGenericRepository<ProductChannelAllocation>
            _repository;

        private readonly IGenericRepository<Product>
            _productRepository;

        private readonly IGenericRepository<SalesChannel>
            _channelRepository;

        private readonly ILogger<ProductChannelAllocationService>
            _logger;

        public ProductChannelAllocationService(
            IUnitOfWork unitOfWork,
            ILogger<ProductChannelAllocationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository =
                _unitOfWork.Repository<ProductChannelAllocation>();

            _productRepository =
                _unitOfWork.Repository<Product>();

            _channelRepository =
                _unitOfWork.Repository<SalesChannel>();
        }

        #region Create

        public async Task<ResponseDto<ProductChannelAllocationDto>>
            CreateAllocation(CreateProductChannelAllocationDto dto)
        {
            try
            {
                if (dto == null)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("اطلاعات ارسالی معتبر نیست.");

                if (dto.ProductId <= 0 || dto.ChannelId <= 0)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("محصول و کانال فروش معتبر نیستند.");

                if (dto.AllocatedQuantity <= 0)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("تعداد تخصیص باید بیشتر از صفر باشد.");

                var product =
                    await _productRepository.GetByIdAsync(dto.ProductId);

                if (product == null)
                    return ResponseDto<ProductChannelAllocationDto>
                        .NotFound("محصول موردنظر یافت نشد.");

                var channel =
                    await _channelRepository.GetByIdAsync(dto.ChannelId);

                if (channel == null)
                    return ResponseDto<ProductChannelAllocationDto>
                        .NotFound("کانال فروش موردنظر یافت نشد.");

                if (!channel.IsActive)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("کانال فروش غیرفعال است.");

                var allocations = await _repository.GetAllAsync();

                bool alreadyExists = allocations.Any(x =>
                    x.ProductId == dto.ProductId &&
                    x.ChannelId == dto.ChannelId);

                if (alreadyExists)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("برای این محصول در این کانال قبلاً تخصیص ثبت شده است.", 409);

                var allocation = new ProductChannelAllocation
                {
                    ProductId = dto.ProductId,
                    ChannelId = dto.ChannelId,
                    AllocatedQuantity = dto.AllocatedQuantity,
                    SoldQuantity = 0
                };

                await _repository.AddAsync(allocation);

                await _unitOfWork.SaveChangesAsync();

                return new ResponseDto<ProductChannelAllocationDto>(
                    MapToDto(allocation),
                    "تخصیص کالا به کانال فروش با موفقیت انجام شد.",
                    201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product channel allocation.");

                return ResponseDto<ProductChannelAllocationDto>.Error(
                    "در ثبت تخصیص کالا خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Update

        public async Task<ResponseDto<ProductChannelAllocationDto>>
            UpdateAllocation(UpdateProductChannelAllocationDto dto)
        {
            try
            {
                if (dto == null)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("اطلاعات ارسالی معتبر نیست.");

                if (dto.AllocatedQuantity <= 0)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("تعداد تخصیص باید بیشتر از صفر باشد.");

                var allocation =
                    await _repository.GetByIdAsync(dto.Id);

                if (allocation == null)
                    return ResponseDto<ProductChannelAllocationDto>
                        .NotFound("تخصیص موردنظر یافت نشد.");

                if (dto.AllocatedQuantity < allocation.SoldQuantity)
                    return ResponseDto<ProductChannelAllocationDto>
                        .Error("تعداد تخصیص نمی‌تواند کمتر از تعداد فروخته‌شده باشد.");

                allocation.AllocatedQuantity = dto.AllocatedQuantity;

                _repository.Update(allocation);

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<ProductChannelAllocationDto>.Success(
                    MapToDto(allocation),
                    "تخصیص کالا با موفقیت ویرایش شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating allocation with ID {Id}.",
                    dto?.Id);

                return ResponseDto<ProductChannelAllocationDto>.Error(
                    "در ویرایش تخصیص کالا خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Delete

        public async Task<ResponseDto<bool>> DeleteAllocation(int id)
        {
            try
            {
                var allocation = await _repository.GetByIdAsync(id);

                if (allocation == null)
                    return ResponseDto<bool>.NotFound(
                        "تخصیص موردنظر یافت نشد.");

                if (allocation.SoldQuantity > 0)
                    return ResponseDto<bool>.Error(
                        "این تخصیص دارای سابقه فروش است و امکان حذف آن وجود ندارد.",
                        409);

                _repository.Delete(allocation);

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true,
                    "تخصیص با موفقیت حذف شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting allocation with ID {Id}.",
                    id);

                return ResponseDto<bool>.Error(
                    "در حذف تخصیص خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get All

        public async Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllAllocations()
        {
            try
            {
                var allocations = await _repository.GetAllAsync();

                var result = allocations
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<List<ProductChannelAllocationDto>>.Success(
                    result,
                    "لیست تخصیص‌های کالا دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting allocations.");

                return ResponseDto<List<ProductChannelAllocationDto>>.Error(
                    "در دریافت لیست تخصیص‌ها خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get By Id

        public async Task<ResponseDto<ProductChannelAllocationDto>>
            GetAllocationById(int id)
        {
            try
            {
                var allocation = await _repository.GetByIdAsync(id);

                if (allocation == null)
                    return ResponseDto<ProductChannelAllocationDto>.NotFound(
                        "تخصیص موردنظر یافت نشد.");

                return ResponseDto<ProductChannelAllocationDto>.Success(
                    MapToDto(allocation),
                    "اطلاعات تخصیص دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting allocation with ID {Id}.",
                    id);

                return ResponseDto<ProductChannelAllocationDto>.Error(
                    "در دریافت اطلاعات تخصیص خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get By Product Id

        public async Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllocationsByProductId(int productId)
        {
            try
            {
                var allocations = await _repository.GetAllAsync();

                var result = allocations
                    .Where(x => x.ProductId == productId)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<List<ProductChannelAllocationDto>>.Success(
                    result,
                    "تخصیص‌های محصول دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting allocations for product {ProductId}.",
                    productId);

                return ResponseDto<List<ProductChannelAllocationDto>>.Error(
                    "در دریافت تخصیص‌های محصول خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Get By Channel Id

        public async Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllocationsByChannelId(int channelId)
        {
            try
            {
                var allocations = await _repository.GetAllAsync();

                var result = allocations
                    .Where(x => x.ChannelId == channelId)
                    .Select(MapToDto)
                    .ToList();

                return ResponseDto<List<ProductChannelAllocationDto>>.Success(
                    result,
                    "تخصیص‌های کانال فروش دریافت شد.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting allocations for channel {ChannelId}.",
                    channelId);

                return ResponseDto<List<ProductChannelAllocationDto>>.Error(
                    "در دریافت تخصیص‌های کانال خطایی رخ داد.",
                    500);
            }
        }

        #endregion

        #region Mapping

        private static ProductChannelAllocationDto MapToDto(
            ProductChannelAllocation allocation)
        {
            return new ProductChannelAllocationDto
            {
                Id = allocation.Id,
                ProductId = allocation.ProductId,
                ChannelId = allocation.ChannelId,
                AllocatedQuantity = allocation.AllocatedQuantity,
                SoldQuantity = allocation.SoldQuantity,
                AvailableQuantity = allocation.AvailableQuantity
            };
        }

        #endregion
    }
}
