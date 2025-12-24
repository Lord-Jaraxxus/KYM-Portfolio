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
    public struct EquipmentStat 
    {
        public int Attack;
        public int Defense;
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "PROJECT KYM/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public int ItemCount;   // 얘는 여기선 좀 빠져야;

        public string ItemName;
        public ItemCategory ItemCategory;
        public Sprite Icon; 
        public string Description;
        public int Price;

        public ItemActionType ItemActionType;
        public EquipSlotType EquipSlotType;
        public EquipmentStat EquipmentStat;
    }
}
