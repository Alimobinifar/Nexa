using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.WarehouseInventory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IWarehouseInventory
{
    public interface IWarehouseInventoryService
    {
        Task<ResponseDto<List<WarehouseInventoryDto>>> GetAllInventories();

        Task<ResponseDto<WarehouseInventoryDto>> GetInventoryById(int id);

        Task<ResponseDto<WarehouseInventoryDto>> GetInventoryByProductId(int productId);
    }
}
