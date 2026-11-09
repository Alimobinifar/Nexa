using Domain.Entities.BaseEntities;
using Domain.Entities.SellingChannels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SalesChannels
{
    public class ProductChannelAllocation : BaseModel
    {
        // سهمیه اولیه اختصاص‌یافته به کانال
        public int AllocatedQuantity { get; set; }

        // تعداد فروخته‌شده از سهمیه
        public int SoldQuantity { get; set; }

        // سهمیه باقی‌مانده
        public int AvailableQuantity => 
            AllocatedQuantity - SoldQuantity;
          
        #region Navigation Properties

        public int ChannelId { get; set; }

        public SalesChannel SalesChannel { get; set; } = null!;

        public int ProductId { get; set; }

        public Domain.Entities.Products.Product Product { get; set; } = null!;
         
        #endregion
    }
}
