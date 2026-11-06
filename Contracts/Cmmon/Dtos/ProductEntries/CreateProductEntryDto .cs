using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductEntries
{
    public class CreateProductEntryDto
    {
        public DateTime EntryDate { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPurchasePrice { get; set; }

        public string? Notes { get; set; }

        public int ProductId { get; set; }

        public int? SupplierId { get; set; }
    }
}
