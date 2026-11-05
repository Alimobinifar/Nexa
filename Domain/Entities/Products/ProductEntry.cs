using Domain.Entities.BaseEntities;
using Domain.Entities.Suppliers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Products
{
    public sealed class ProductEntry : BaseModel
    {
        public string PersianDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPurchasePrice { get; set; }
        public decimal TotalValue => Quantity * UnitPurchasePrice;
        public string? Notes { get; set; }

        #region NavigationProperty
        public int? SupplierId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Supplier Supplier { get; set; }
        #endregion
    }
}
