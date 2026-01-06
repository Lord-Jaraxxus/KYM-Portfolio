using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class DropItem : MonoBehaviour, IInteractable
    {
        [SerializeField] public ItemDataSO itemData; // 획득 가능한 아이템 데이터
        public int Quantity => quantity;
        [SerializeField] private int quantity = 1; // 획득 가능한 아이템 수량
        public InteractableType Type { get; } = InteractableType.DropItem;

        public void Interact()
        {
            // 아이템 획득 로직 구현
            Debug.Log("아이템을 획득했습니다!");
            Destroy(gameObject); // 아이템 오브젝트 제거

            UserDataModel.Singleton.AddItem(itemData.ItemID, quantity);
        }

        public void Initialize(ItemDataSO itemDataSO, int quantity) 
        {
            this.itemData = itemDataSO;
            this.quantity = quantity;
        }

        public Transform GetTransform() => this.transform;
    }
}
