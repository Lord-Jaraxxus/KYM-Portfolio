using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "ShopData", menuName = "PROJECT KYM/ShopData")]
    public class ShopDataSO : ScriptableObject
    {
        public string ShopID;
        public string ShopName;
        public List<ItemDataSO> ItemsForSale;
    }
}
