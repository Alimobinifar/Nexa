using Contracts.Cmmon.Dtos.Product;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Products
{
    public interface IProductService
    {
        Task<ResponseDto<CreateProductDto>> CreateProduct(CreateProductDto product);
        Task<ResponseDto<UpdateProductDto>> UpdateProduct(UpdateProductDto product);
        Task<ResponseDto<bool>> DeleteProduct(int productId);
        Task<ResponseDto<IEnumerable<ProductDto>>> ProductsList();
    }
}
