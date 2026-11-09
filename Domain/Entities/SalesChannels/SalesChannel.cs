using Domain.Entities.BaseEntities;
using Domain.Entities.SalesChannels;
using Domain.Entities.Salse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SellingChannels
{
    public sealed class SalesChannel : BaseModel
    {
        // نام کانال فروش
        public string Name { get; set; } = string.Empty;

        // توضیحات کانال
        public string? Description { get; set; }

        // درصد کمیسیون پیش‌فرض
        public decimal DefaultCommissionRate { get; set; }

        // هزینه ثابت هر ثبت فروش
        public decimal FixedFee { get; set; }

        // آدرس وب‌سایت یا پنل کانال
        public string? WebsiteUrl { get; set; }

        // شماره پشتیبانی
        public string? SupportPhone { get; set; }

        // ترتیب نمایش
        public int DisplayOrder { get; set; }

        #region Navigation Properties
        public ICollection<SaleRegistration> SaleRegistrations
        { get; set; } = new List<SaleRegistration>();
        public ICollection<ProductChannelAllocation>
        ProductChannelAllocations
        { get; set; } = new List<ProductChannelAllocation>();

        #endregion
    }
}