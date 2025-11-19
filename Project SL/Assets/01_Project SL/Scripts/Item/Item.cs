using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Item 
    {
        public ItemDataSO itemData;
        public int quantity;
        public Item(ItemDataSO itemData, int quantity = 1)
        {
            this.itemData = itemData;
            this.quantity = quantity;
        }
    }
}
