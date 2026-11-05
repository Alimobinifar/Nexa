using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.SaleRegistration
{
    public class CreateSaleRegistrationDto
    {
        public int ProductQuantity { get; set; }

        public decimal SalePrice { get; set; }

        public decimal BuyPrice { get; set; }

        public decimal SendPrice { get; set; }

        public decimal PackingPrice { get; set; }

        public decimal CommissionRate { get; set; }

        public int ChannelId { get; set; }

        public int ProductId { get; set; }
    }
}
