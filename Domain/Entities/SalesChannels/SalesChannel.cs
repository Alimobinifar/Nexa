using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.SellingChannels
{
    public sealed class SalesChannel:BaseModel
    {
        public string Name { get; set; } = "";
        public decimal DefaultCommissionRate { get; set; } 
    }
}