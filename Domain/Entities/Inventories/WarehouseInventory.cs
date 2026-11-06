using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Products;

namespace Domain.Entities.Inventories
{
    public sealed class WarehouseInventory : BaseModel
    {
        public decimal CurrentInventory { get; set; }

        #region NavigationProperty And Relations

        public int ProductId { get; set; }
        public Domain.Entities.Products.Product Product { get; set; } = null!;

        #endregion
    }
}
