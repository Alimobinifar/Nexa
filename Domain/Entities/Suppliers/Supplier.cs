using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Domain.Entities.Suppliers
{
    public class Supplier:BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactNumber { get; set; }
    }
}
