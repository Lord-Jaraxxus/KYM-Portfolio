using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace KYM
{
    public class CharacterEquipment : MonoBehaviour
    {
        public EquipSlotType equipSlotType; // 장비 슬롯 타입
        public ItemDataSO itemDataSO; // 장착된 아이템 데이터SO

        // 나중에 모델링 갈아끼울때를 위한 변수
        public GameObject equipModeling; // 모델링? 프리펩?
        public Transform equipPoint; // 장비가 장착될 트랜스폼

        public void ChangeEquipment(ItemDataSO beforeEquipSO, ItemDataSO newEqiupSO) 
        {
            itemDataSO = newEqiupSO;

            // TODO : 모델링 갈아끼우기, 스텟 변경?
        }
    }
}
