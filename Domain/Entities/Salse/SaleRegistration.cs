using Domain.Entities.BaseEntities;
using Domain.Entities.SalesChannels;
using Domain.Entities.SellingChannels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Salse
{
    public class SaleRegistration : BaseModel
    {
        // تعداد محصول فروخته‌شده
        public int ProductQuantity { get; set; }

        // قیمت فروش هر واحد
        public decimal SalePrice { get; set; }

        // قیمت خرید هر واحد در زمان فروش
        public decimal BuyPrice { get; set; }

        // هزینه ارسال کل این ثبت فروش
        public decimal SendPrice { get; set; }

        // هزینه بسته‌بندی کل این ثبت فروش
        public decimal PackingPrice { get; set; }

        // درصد کمیسیون کانال فروش
        public decimal CommissionRate { get; set; }

        // مبلغ کل فروش
        public decimal TotalSaleAmount { get; private set; }

        // مبلغ کل خرید
        public decimal PurchaseNetCost { get; private set; }

        // مبلغ کمیسیون
        public decimal CommissionCost { get; private set; }

        // سود خالص
        public decimal NetProfit { get; private set; }

        // درصد حاشیه سود خالص
        public decimal NetProfitPercent { get; private set; }

        #region Navigation Properties

        public int ChannelId { get; set; }
        public SalesChannel SalesChannel { get; set; } = null!;

        public int ProductId { get; set; }
        public Domain.Entities.Products.Product Product { get; set; } = null!;


        #endregion

        #region Financial Calculations

        public void CalculateFinancials()
        {
            if (ProductQuantity <= 0)
                throw new InvalidOperationException(
                    "تعداد محصول باید بیشتر از صفر باشد.");

            if (SalePrice < 0 || BuyPrice < 0 ||
                SendPrice < 0 || PackingPrice < 0)
                throw new InvalidOperationException(
                    "مقادیر قیمت و هزینه نمی‌توانند منفی باشند.");

            if (CommissionRate < 0 || CommissionRate > 100)
                throw new InvalidOperationException(
                    "درصد کمیسیون باید بین صفر تا صد باشد.");

            TotalSaleAmount = ProductQuantity * SalePrice;

            PurchaseNetCost = ProductQuantity * BuyPrice;

            CommissionCost =
                TotalSaleAmount * CommissionRate / 100m;

            NetProfit =
                TotalSaleAmount
                - PurchaseNetCost
                - CommissionCost
                - SendPrice
                - PackingPrice;

            NetProfitPercent = TotalSaleAmount > 0
                ? (NetProfit / TotalSaleAmount) * 100m
                : 0;
        }

        #endregion
    }
}
