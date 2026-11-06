using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.ProductChannelAllocation
{
    public class UpdateProductChannelAllocationDto
    {
        public int Id { get; set; }

        public int AllocatedQuantity { get; set; }
    }
}
