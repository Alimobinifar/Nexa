using Domain.Entities.BaseEntities;
using Domain.Entities.SellingChannels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SalesChannels
{
    public class ProductChannelAllocation:BaseModel
    {
        public int QuantitiesByChannel { get; set; }

        #region NavigationProperty And Relations
        public  int SaleChannelId { get; set; }
        public SalesChannel SalesChannel { get; set; }
        public int ProductId { get; set; }
        public Domain.Entities.Products.Product Product { get; set; }
        #endregion
    }
}
