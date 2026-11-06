using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductChannelAllocation
{
    public class CreateProductChannelAllocationDto
    {
        public int ProductId { get; set; }

        public int ChannelId { get; set; }

        public int AllocatedQuantity { get; set; }
    }
}
