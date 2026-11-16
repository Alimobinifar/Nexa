using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Nexa.Front.Services.Products.Interfaces;

namespace Nexa.Front.Services.Products
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;

        public ProductApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseDto<ProductDto>> CreateProductAsync(
            CreateProductDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "Product/Create", dto);
               

            var result = await response.Content
                .ReadFromJsonAsync<ResponseDto<ProductDto>>();

            return result ?? ResponseDto<ProductDto>.Error(
                "پاسخی از سرور دریافت نشد",
                500);
        }
    }
}
