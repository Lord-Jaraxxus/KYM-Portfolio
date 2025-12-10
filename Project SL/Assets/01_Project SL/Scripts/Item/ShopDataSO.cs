using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "ShopData", menuName = "PROJECT KYM/ShopData")]
    public class ShopDataSO : ScriptableObject
    {
        public int shopID;
        public string shopName;
        public List<ItemDataSO> itemsForSale;
    }
}
