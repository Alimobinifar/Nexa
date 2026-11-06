using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Cmmon.Dtos.WarehouseInventory
{
    public class WarehouseInventoryDto
    {
        public int Id { get; set; }

        public decimal CurrentInventory { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
    }   
}
