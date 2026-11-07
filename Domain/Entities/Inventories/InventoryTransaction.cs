using Domain.Entities.BaseEntities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Inventories
{
    public sealed class InventoryTransaction : BaseModel
    {
        public int ProductId { get; set; }
        public Domain.Entities.Products.Product Product { get; set; } = null!;

        public InventoryTransactionType TransactionType { get; set; }

        public decimal Quantity { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string ReferenceType { get; set; } = string.Empty;

        public int ReferenceId { get; set; }

        public string? Description { get; set; }
    }
}
