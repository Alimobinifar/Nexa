using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductChannelAllocation
{
    public class ProductChannelAllocationDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int ChannelId { get; set; }

        public int AllocatedQuantity { get; set; }

        public int SoldQuantity { get; set; }

        public int AvailableQuantity { get; set; }
    }
}
