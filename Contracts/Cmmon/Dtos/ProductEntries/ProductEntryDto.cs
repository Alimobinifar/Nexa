using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductEntries
{
    public class ProductEntryDto
    {
        public int Id { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPurchasePrice { get; set; }

        public decimal TotalValue { get; set; }

        public string? Notes { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int? SupplierId { get; set; }

        public string? SupplierName { get; set; }
    }
}
