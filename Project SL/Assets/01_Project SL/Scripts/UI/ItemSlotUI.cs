using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class ItemSlotUI : UIBase
    {
        Image backgorund;
        Image icon;
        public TextMeshProUGUI itemName;

        public void SetData(ItemDataSO itemSO) 
        {
            icon.sprite = itemSO.icon;
            itemName.text = itemSO.itemName;
        }
    }
}

