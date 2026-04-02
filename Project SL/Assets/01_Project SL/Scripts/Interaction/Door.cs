using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Door : MonoBehaviour, IInteractable
    {
        public InteractableType type => InteractableType.Door;

        public Transform GetTransform()
        {
            return this.transform;
        }

        public void Interact()
        {
            if (UserDataModel.Singleton.PlayerItemDto.PlayerItems.Find(x => x.ItemID == "key") == null)
            {
                // 열쇠 아이템이 없는 경우
                Debug.Log("열쇠가 필요합니다!");
                return;
            }
            else
            {
                // 문이 열리는 로직 구현
                Debug.Log("문이 열렸습니다!");

                UserDataModel.Singleton.RemoveItem("key", 1); // 열쇠 아이템 제거 (1개 소모)

                this.transform.gameObject.SetActive(false); // 문 오브젝트 비활성화 (열린 상태로 변경) -> 일단 임시로 

                return;
            }
        }
    }
}
