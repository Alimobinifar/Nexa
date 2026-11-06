using Contracts.Cmmon.Dtos.ProductChannelAllocation;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.ProductAllocations
{
    public interface IProductChannelAllocationService
    {
        Task<ResponseDto<ProductChannelAllocationDto>>
            CreateAllocation(CreateProductChannelAllocationDto dto);

        Task<ResponseDto<ProductChannelAllocationDto>>
            UpdateAllocation(UpdateProductChannelAllocationDto dto);

        Task<ResponseDto<bool>>
            DeleteAllocation(int id);

        Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllAllocations();

        Task<ResponseDto<ProductChannelAllocationDto>>
            GetAllocationById(int id);

        Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllocationsByProductId(int productId);

        Task<ResponseDto<List<ProductChannelAllocationDto>>>
            GetAllocationsByChannelId(int channelId);
    }
}
