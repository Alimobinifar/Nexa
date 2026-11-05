using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Product
{
    public class ProductDto
    {
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId {  get; set; }
        public string? ProductDescription { get; set; }
    }
}
