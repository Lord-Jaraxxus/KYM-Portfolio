using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface IItemAction { }

    public interface IUsableItem : IItemAction
    {
        bool CanUse(CharacterBase character);
        void Use(CharacterBase character);
    }

    public interface IEquipableItem : IItemAction 
    {
        EquipSlotType SlotType { get; }
        void Equip(CharacterBase character);
        void Unequip(CharacterBase character);
    }

}
