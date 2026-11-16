using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;

namespace Nexa.Front.Services.Products.Interfaces
{
    public interface IProductApiService
    {
        Task<ResponseDto<ProductDto>> CreateProductAsync(
                 CreateProductDto dto);

    }
}
