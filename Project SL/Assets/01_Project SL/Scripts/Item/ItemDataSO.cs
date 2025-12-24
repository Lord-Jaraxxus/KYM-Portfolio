using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public enum ItemCategory
    {
        All = 0,
        Equipment = 1,
        Material = 2,
        Consumable = 3,
        Quest = 4,
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "PROJECT KYM/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public int ItemCount;

        public string ItemName;
        public ItemCategory ItemCategory;
        public Sprite Icon;
        public string Description;
        public int Price;
    }
}
