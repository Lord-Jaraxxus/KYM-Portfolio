using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface IItemAction { }

    public interface IUsableItem : IItemAction
    {
        ItemDataSO ItemDataSO { get; }

        bool CanUse(CharacterBase character);
        void Use(CharacterBase character);
    }

    public interface IEquipableItem : IItemAction 
    {
        ItemDataSO ItemDataSO { get; }

        void Equip(CharacterBase character);
        void Unequip(CharacterBase character);
    }

}
