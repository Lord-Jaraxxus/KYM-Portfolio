using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "CharacterStatData", menuName = "PROJECT KYM/ItemData")]
    public class ItemDataSO : ScriptableObject
    {
        public string ID;
        public int ItemCount;

        public string itemName;
        public Sprite icon;
        public string description;
    }
}
