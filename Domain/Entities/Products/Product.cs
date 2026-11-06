using Domain.Entities.BaseEntities;
using Domain.Entities.Product;
using Domain.Entities.SalesChannels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities.Products
{
    public class Product:BaseModel
    {
        [Required]
        public string ProductName {  get; set; }
        public string? ProductDescription { get; set; }
        
        
        #region NavigationProperty And Relations
        public int CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ICollection<ProductChannelAllocation> ChannelAllocations { get; set; }
    = new List<ProductChannelAllocation>();
        #endregion
    }
}
