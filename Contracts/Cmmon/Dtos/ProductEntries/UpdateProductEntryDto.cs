using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductEntries
{
    public class UpdateProductEntryDto
    {
        public int Id { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPurchasePrice { get; set; }

        public string? Notes { get; set; }

        public int ProductId { get; set; }

        public int? SupplierId { get; set; }
    }
}
