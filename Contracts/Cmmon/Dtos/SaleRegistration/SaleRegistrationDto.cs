using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.SaleRegistration
{
    public class SaleRegistrationDto
    {
        public int Id { get; set; }

        public int ProductQuantity { get; set; }

        public decimal SalePrice { get; set; }

        public decimal BuyPrice { get; set; }

        public decimal SendPrice { get; set; }

        public decimal PackingPrice { get; set; }

        public decimal CommissionRate { get; set; }

        public decimal TotalSaleAmount { get; set; }

        public decimal PurchaseNetCost { get; set; }

        public decimal CommissionCost { get; set; }

        public decimal NetProfit { get; set; }

        public decimal NetProfitPercent { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int ChannelId { get; set; }

        public string ChannelName { get; set; } = string.Empty;
    }
}
