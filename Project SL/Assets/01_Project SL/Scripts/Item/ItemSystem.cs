using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public static class ItemSystem
    {
        public static void UseItem(string itemID) 
        {
            // TODO : 아이템 사용, 즉 아이템 갯수를 1개 줄이고 0개가 되면 인벤토리에서 없애기 + 장비면 장착, 소모품이면 효과 적용 해야함...;

            ItemDataSO itemDataSO = GameDataModel.Singleton.GetItemDataSO(itemID); // ID를 통해 아이템데이터베이스에서 ItemSO를 가져옴
            IItemAction itemAction = ItemActionFactory.Create(itemDataSO); // 아이템 액션 인스턴스 생성
            CharacterBase playerCharacter = PlayerController.Instance.LinkedCharacter; // 현재 플레이어 캐릭터 가져옴


            if (itemAction is IUsableItem usable) // 사용한 아이템이 소모품일 경우
            {
                UserDataModel.Singleton.RemoveItem(itemID, 1); // 일단 UDM에서 갯수 하나 줄임
                // usable.Use(playerCharacter); 이건 좀 나중에 구현
            }
            else if (itemAction is IEquipableItem equipable) // 사용한 아이템이 장비템일 경우
            {
                UserDataModel.Singleton.RemoveItem(itemID, 1); // 장비템도 일단 UDM에서 갯수 하나 줄임
                equipable.Equip(playerCharacter);
            }
            else // 사용 불가 아이템일 경우
            {
                Debug.Log("사용 불가 아이템을 사용하셨습니다!");
            }
        }

        public static void DropItem(string itemID)
        {
            // TODO : 아이템 버리기, 즉 아이템 갯수를 1개 줄이고 0개가 되면 인벤토리에서 없애기 + 버려진 아이템을 월드에 드랍하기;
            ItemDataSO itemDataSO = GameDataModel.Singleton.GetItemDataSO(itemID); // ID를 통해 아이템데이터베이스에서 ItemSO를 가져옴
            int quantity = UserDataModel.Singleton.PlayerItemDto.PlayerItems.Find(item => item.ItemID == itemID).ItemCount;

            UserDataModel.Singleton.RemoveItem(itemID, quantity); // UDM에서 모든 갯수 줄임 (일단은 전부 버리는걸로)

            // 1. 플레이어 위치 가져오기
            Vector3 dropPosition = PlayerController.Instance.LinkedCharacter.transform.position;

            // 2. 프리팹 소환
            GameObject itemVisual = GameObject.Instantiate(
                itemDataSO.ItemVisualPrefab,
                dropPosition,
                Quaternion.identity);

            // 3. DropItem 컴포넌트 초기화
            DropItem dropItemComp = itemVisual.AddComponent<DropItem>();
            dropItemComp.Initialize(itemDataSO, quantity);
        }
    }
}
