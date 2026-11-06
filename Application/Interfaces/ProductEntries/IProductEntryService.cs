using Contracts.Cmmon.Dtos.ProductEntries;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.ProductEntries
{
    public interface IProductEntryService
    {
        Task<ResponseDto<ProductEntryDto>> CreateProductEntry(CreateProductEntryDto dto);

        Task<ResponseDto<ProductEntryDto>> UpdateProductEntry(UpdateProductEntryDto dto);

        Task<ResponseDto<bool>> DeleteProductEntry(int id);

        Task<ResponseDto<List<ProductEntryDto>>> GetAllProductEntries();

        Task<ResponseDto<ProductEntryDto>> GetProductEntryById(int id);

        Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesByProductId(int productId);

        Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesBySupplierId(int supplierId);

        Task<ResponseDto<List<ProductEntryDto>>> GetProductEntriesByDateRange(DateTime startDate, DateTime endDate);
    }
}
