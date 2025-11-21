using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(menuName = "Inventory/Item")]
    public class ItemDataSO : ScriptableObject
    {
        public string ID;
        public string itemName;
        public Sprite icon;
        public string description;
    }
}
