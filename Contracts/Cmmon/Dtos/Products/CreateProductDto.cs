using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Contracts.Cmmon.Dtos.Product
{
    public class CreateProductDto
    {
        [Required]
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public string? ProductDescription { get; set; }
    }
}
