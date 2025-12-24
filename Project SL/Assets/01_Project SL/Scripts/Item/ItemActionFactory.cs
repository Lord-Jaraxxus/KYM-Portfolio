using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public static class ItemActionFactory
    {
        public static IItemAction Create(ItemDataSO itemDataSO) 
        {
            switch (itemDataSO.ItemActionType)
            {
                case ItemActionType.Equip:
                    return new EquipableItemAction(itemDataSO);

                case ItemActionType.Potion:
                    return null;
                default: 
                    return null;
            }
        }
    }
}
