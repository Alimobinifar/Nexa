using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Suppliers
{
    public class UpdateSupplierDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;
    }
}
