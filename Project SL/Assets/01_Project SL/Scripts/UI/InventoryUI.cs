using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KYM
{
    public class InventoryUI : UIBase
    {
        [SerializeField] GameObject itemSlotPrefab;

        ItemSlotUI[] itemSlots;
        Inventory inventory;
        

        private void Start()
        {
            inventory.OnInventoryChanged += UpdateUI;
        }

        public void UpdateUI()
        {
            var items = inventory.itemDictionary.Values;

            GameObject newSlot = Instantiate(itemSlotPrefab, transform);
        }
    }
}
