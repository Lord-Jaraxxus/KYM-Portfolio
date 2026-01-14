using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [System.Serializable]
    public class  ShopItemEntry
    {
        public ItemDataSO itemDataSO;
        public int initialStock;
    }

    [CreateAssetMenu(fileName = "ShopData", menuName = "PROJECT KYM/ShopData")]
    public class ShopDataSO : ScriptableObject
    {
        public string ShopID;
        public string ShopName;
        public List<ItemDataSO> ItemsForSale;   // 원래 쓰던 리스트
        public List<ShopItemEntry> ItemEntries;    // 새로 추가한 리스트
    }
}
