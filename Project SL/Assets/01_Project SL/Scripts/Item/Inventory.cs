using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Inventory : MonoBehaviour
    {
        public event Action OnInventoryChanged;
        public Dictionary<ItemDataSO, Item> itemDictionary = new Dictionary<ItemDataSO, Item>();


        public void AddItem(Item item)
        { 
            OnInventoryChanged?.Invoke();
        }
    }
}
