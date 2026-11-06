using Domain.Entities.BaseEntities;
using Domain.Entities.Suppliers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Products
{
    public sealed class ProductEntry : BaseModel
    {
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;

        public decimal Quantity { get; set; }

        public decimal UnitPurchasePrice { get; set; }

        public decimal TotalValue => Quantity * UnitPurchasePrice;

        public string? Notes { get; set; }

        #region Navigation Properties

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int? SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        #endregion
    }
}
