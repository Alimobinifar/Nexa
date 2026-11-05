using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Products;

namespace Domain.Entities.Inventories
{
    public class Inventory:BaseModel
    {
        public int CurrentInventory {  get; set; }
        
        #region NavigationProperty And Relations
        public int ProductId { get; set; }
        public Domain.Entities.Products.Product Product { get; set; }
        #endregion
        }
}
