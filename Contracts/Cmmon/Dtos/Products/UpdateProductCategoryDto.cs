using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.Product
{
    public class UpdateProductCategoryDto
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int? ParentId { get; set; }
    }
}
