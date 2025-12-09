using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class DropItem : MonoBehaviour, IInteractable
    {
        [SerializeField] public ItemDataSO itemData; // 획득 가능한 아이템 데이터
        public InteractableType Type { get; } = InteractableType.DropItem;

        public void Interact()
        {
            // 아이템 획득 로직 구현
            Debug.Log("아이템을 획득했습니다!");
            Destroy(gameObject); // 아이템 오브젝트 제거

            UserDataModel.Singleton.AddItem(itemData.ID, itemData.ItemCount);
        }

        public Transform GetTransform() => this.transform;
    }
}
