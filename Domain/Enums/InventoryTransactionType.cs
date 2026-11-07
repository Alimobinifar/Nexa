using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum InventoryTransactionType
    {
        PurchaseEntry = 1,
        Sale = 2,
        SaleReturn = 3,
        PurchaseReturn = 4,
        AdjustmentIncrease = 5,
        AdjustmentDecrease = 6
    }
}
