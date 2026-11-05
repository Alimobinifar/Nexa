using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Product
{

    public class ProductCategory : BaseModel
    {
        public string CategoryCode { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;


        // Self-referencing relationship
        public int? ParentId { get; set; }
        public ProductCategory? ParentCategory { get; set; }
        public ICollection<ProductCategory> SubCategories { get; set; }
            = new List<ProductCategory>();
    }

}
