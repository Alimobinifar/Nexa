using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Contracts.Cmmon.Dtos.Product
{
    public class UpdateProductDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public string? ProductDescription { get; set; }
    }
}
