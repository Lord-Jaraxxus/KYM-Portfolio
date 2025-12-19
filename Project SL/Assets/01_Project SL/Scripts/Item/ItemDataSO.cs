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
        Equipment_Weapon,
        Equipment_Helmet,
        Equipment_Armor,
        Equipment_Pants,
        Equipment_Boots,
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "PROJECT KYM/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public int ItemCount;

        public string ItemName;
        public ItemCategory Category;
        public Sprite Icon;
        public string Description;
        public int Price;
    }
}
