using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.SaleRegistration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.SalesRegistration
{
    public interface ISaleRegistrationService
    {
        Task<ResponseDto<SaleRegistrationDto>> CreateSaleRegistration(
            CreateSaleRegistrationDto dto);

        Task<ResponseDto<SaleRegistrationDto>> UpdateSaleRegistration(
            UpdateSaleRegistrationDto dto);

        Task<ResponseDto<bool>> DeleteSaleRegistration(int id);

        Task<ResponseDto<IEnumerable<SaleRegistrationDto>>> GetAllSaleRegistrations();

        Task<ResponseDto<SaleRegistrationDto>> GetSaleRegistrationById(int id);

        Task<ResponseDto<IEnumerable<SaleRegistrationDto>>> GetByProductId(int productId);

        Task<ResponseDto<IEnumerable<SaleRegistrationDto>>> GetByChannelId(int channelId);
    }
}
