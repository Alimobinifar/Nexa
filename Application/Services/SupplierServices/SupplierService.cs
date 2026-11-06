using Application.Interfaces.Suppliers;
using Contracts.Cmmon.Dtos.ResultAndResponse;
using Contracts.Cmmon.Dtos.Suppliers;
using Domain.Entities.Suppliers;
using Domain.Interfaces.GenericInterfaces;
using Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.SupplierServices
{

    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Supplier> _repository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(
            IUnitOfWork unitOfWork,
            ILogger<SupplierService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            _repository = _unitOfWork.Repository<Supplier>();
        }

        // Create Supplier
        public async Task<ResponseDto<SupplierDto>> CreateSupplier(
            CreateSupplierDto supplier)
        {
            try
            {
                if (supplier == null)
                    return ResponseDto<SupplierDto>.Error(
                        "Supplier data is required.", 400);

                if (string.IsNullOrWhiteSpace(supplier.Name))
                    return ResponseDto<SupplierDto>.Error(
                        "Supplier name is required.", 400);

                if (string.IsNullOrWhiteSpace(supplier.ContactNumber))
                    return ResponseDto<SupplierDto>.Error(
                        "Contact number is required.", 400);

                var newSupplier = new Supplier
                {
                    Name = supplier.Name.Trim(),
                    Description = supplier.Description?.Trim() ?? string.Empty,
                    ContactNumber = supplier.ContactNumber.Trim()
                };

                await _repository.AddAsync(newSupplier);
                await _unitOfWork.SaveChangesAsync();

                var result = new SupplierDto
                {
                    Id = newSupplier.Id,
                    Name = newSupplier.Name,
                    Description = newSupplier.Description,
                    ContactNumber = newSupplier.ContactNumber
                };

                return ResponseDto<SupplierDto>.Success(
                    result, "Supplier created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier.");

                return ResponseDto<SupplierDto>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        // Update Supplier
        public async Task<ResponseDto<SupplierDto>> UpdateSupplier(
            UpdateSupplierDto supplier)
        {
            try
            {
                if (supplier == null)
                    return ResponseDto<SupplierDto>.Error(
                        "Supplier data is required.", 400);

                if (supplier.Id <= 0)
                    return ResponseDto<SupplierDto>.Error(
                        "Invalid supplier ID.", 400);

                if (string.IsNullOrWhiteSpace(supplier.Name))
                    return ResponseDto<SupplierDto>.Error(
                        "Supplier name is required.", 400);

                if (string.IsNullOrWhiteSpace(supplier.ContactNumber))
                    return ResponseDto<SupplierDto>.Error(
                        "Contact number is required.", 400);

                var existingSupplier = await _repository.GetByIdAsync(supplier.Id);

                if (existingSupplier == null)
                    return ResponseDto<SupplierDto>.NotFound(
                        "Supplier not found.");

                existingSupplier.Name = supplier.Name.Trim();
                existingSupplier.Description =
                    supplier.Description?.Trim() ?? string.Empty;
                existingSupplier.ContactNumber = supplier.ContactNumber.Trim();

                _repository.Update(existingSupplier);

                await _unitOfWork.SaveChangesAsync();

                var result = new SupplierDto
                {
                    Id = existingSupplier.Id,
                    Name = existingSupplier.Name,
                    Description = existingSupplier.Description,
                    ContactNumber = existingSupplier.ContactNumber
                };

                return ResponseDto<SupplierDto>.Success(
                    result, "Supplier updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier.");

                return ResponseDto<SupplierDto>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        // Delete Supplier
        public async Task<ResponseDto<bool>> DeleteSupplier(int supplierId)
        {
            try
            {
                if (supplierId <= 0)
                    return ResponseDto<bool>.Error(
                        "Invalid supplier ID.", 400);

                var supplier = await _repository.GetByIdAsync(supplierId);

                if (supplier == null)
                    return ResponseDto<bool>.NotFound(
                        "Supplier not found.");

                _repository.Delete(supplier);

                await _unitOfWork.SaveChangesAsync();

                return ResponseDto<bool>.Success(
                    true, "Supplier deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier {SupplierId}.",
                    supplierId);

                return ResponseDto<bool>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        // Get All Suppliers
        public async Task<ResponseDto<IEnumerable<SupplierDto>>> GetAllSuppliers()
        {
            try
            {
                var suppliers = (await _repository.GetAllAsync()).ToList();

                if (!suppliers.Any())
                    return ResponseDto<IEnumerable<SupplierDto>>.NotFound(
                        "No suppliers found.");

                var result = suppliers.Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    ContactNumber = s.ContactNumber
                }).ToList();

                return ResponseDto<IEnumerable<SupplierDto>>.Success(
                    result, "Suppliers retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suppliers.");

                return ResponseDto<IEnumerable<SupplierDto>>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        // Get Supplier By Id
        public async Task<ResponseDto<SupplierDto>> GetSupplierById(int supplierId)
        {
            try
            {
                if (supplierId <= 0)
                    return ResponseDto<SupplierDto>.Error(
                        "Invalid supplier ID.", 400);

                var supplier = await _repository.GetByIdAsync(supplierId);

                if (supplier == null)
                    return ResponseDto<SupplierDto>.NotFound(
                        "Supplier not found.");

                var result = new SupplierDto
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    Description = supplier.Description,
                    ContactNumber = supplier.ContactNumber
                };

                return ResponseDto<SupplierDto>.Success(
                    result, "Supplier retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving supplier {SupplierId}.", supplierId);

                return ResponseDto<SupplierDto>.Error(
                    "An unexpected error occurred.", 500);
            }
        }

        // Search Suppliers
        public async Task<ResponseDto<IEnumerable<SupplierDto>>> SearchSuppliers(
            string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return ResponseDto<IEnumerable<SupplierDto>>.Error(
                        "Search keyword is required.", 400);

                var suppliers = (await _repository.GetAllAsync()).ToList();

                var result = suppliers
                    .Where(s =>
                        (s.Name != null &&
                         s.Name.Contains(keyword.Trim(),
                             StringComparison.OrdinalIgnoreCase)) ||

                        (s.ContactNumber != null &&
                         s.ContactNumber.Contains(keyword.Trim())))

                    .Select(s => new SupplierDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        ContactNumber = s.ContactNumber
                    })
                    .ToList();

                if (!result.Any())
                    return ResponseDto<IEnumerable<SupplierDto>>.NotFound(
                        "No matching suppliers found.");

                return ResponseDto<IEnumerable<SupplierDto>>.Success(
                    result, "Suppliers retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error searching suppliers with keyword {Keyword}.", keyword);

                return ResponseDto<IEnumerable<SupplierDto>>.Error(
                    "An unexpected error occurred.", 500);
            }
        }
    }

}
