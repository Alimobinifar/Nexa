using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.SaleChannels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.SaleChannels
{
    public interface ISalesChannelService
    {
        Task<ResponseDto<SalesChannelDto>> CreateSalesChannel(
            CreateSalesChannelDto dto);

        Task<ResponseDto<SalesChannelDto>> UpdateSalesChannel(
            UpdateSalesChannelDto dto);

        Task<ResponseDto<bool>> DeleteSalesChannel(int id);

        Task<ResponseDto<List<SalesChannelDto>>> GetAllSalesChannels();

        Task<ResponseDto<SalesChannelDto>> GetSalesChannelById(int id);

        Task<ResponseDto<List<SalesChannelDto>>> SearchSalesChannels(
            string searchTerm);
    }
}
