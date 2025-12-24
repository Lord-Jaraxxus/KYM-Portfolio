using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class CharacterInfoUI : UIBase
    {
        [SerializeField] private Image EquipIcon_Head;
        [SerializeField] private Image EquipIcon_Body;
        [SerializeField] private Image EquipIcon_Legs;
        [SerializeField] private Image EquipIcon_Weapon;
        [SerializeField] private Image EquipIcon_Shield;

        public void SetIcon(ItemDataSO itemDataSO)
        {
            switch (itemDataSO.EquipSlotType)
            {
                case EquipSlotType.Head:
                    EquipIcon_Head.sprite = itemDataSO.Icon;
                    break;
                case EquipSlotType.Body:
                    EquipIcon_Body.sprite = itemDataSO.Icon;
                    break;
                case EquipSlotType.Legs:
                    EquipIcon_Legs.sprite = itemDataSO.Icon;
                    break;
                case EquipSlotType.Weapon:
                    EquipIcon_Weapon.sprite = itemDataSO.Icon;
                    break;
                case EquipSlotType.Shield:
                    EquipIcon_Shield.sprite = itemDataSO.Icon;
                    break;
                default:
                    break;
            }
        }
    }
}
