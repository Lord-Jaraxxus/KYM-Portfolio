using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class EquipableItemAction : IEquipableItem
    {
        public ItemDataSO ItemDataSO { get; }

        public EquipableItemAction(ItemDataSO itemDataSO)
        {
            if (itemDataSO == null)
            {
                Debug.Log("itemDataSO is null!");
                return;
            }

            this.ItemDataSO = itemDataSO;
        }

        public void Equip(CharacterBase character)
        {
            character.EquipItem(ItemDataSO);
        }

        public void Unequip(CharacterBase character)
        {
            character.UneqipItem(ItemDataSO);
        }
    }
}
