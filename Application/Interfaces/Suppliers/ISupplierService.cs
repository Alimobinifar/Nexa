using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.Suppliers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Suppliers
{
    public interface ISupplierService
    {
        Task<ResponseDto<SupplierDto>> CreateSupplier(
            CreateSupplierDto supplier);

        Task<ResponseDto<SupplierDto>> UpdateSupplier(
            UpdateSupplierDto supplier);

        Task<ResponseDto<bool>> DeleteSupplier(int supplierId);

        Task<ResponseDto<IEnumerable<SupplierDto>>> GetAllSuppliers();

        Task<ResponseDto<SupplierDto>> GetSupplierById(int supplierId);

        Task<ResponseDto<IEnumerable<SupplierDto>>> SearchSuppliers(
            string keyword);
    }
}
