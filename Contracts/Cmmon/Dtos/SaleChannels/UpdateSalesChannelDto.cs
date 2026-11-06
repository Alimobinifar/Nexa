using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.SaleChannels
{
    public class UpdateSalesChannelDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal DefaultCommissionRate { get; set; }

        public decimal FixedFee { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? SupportPhone { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }
    }
}
