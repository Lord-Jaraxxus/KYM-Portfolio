using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Constant { }
    public enum EquipSlotType
    {
        None = 0,
        Head,
        Body,
        Legs,
        Weapon,
        Shield
    }

    public enum ItemActionType 
    {
        None = 0,
        Equip,
        Potion,
    }
}
