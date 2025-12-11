using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KYM
{
    public class InventoryUI : UIBase
    {
        public Gpm.Ui.InfiniteScroll infiniteScroll;
        public GameObject listItemPrefab;

        private Dictionary<string, InfiniteUI_ListData> infiniteDataContainer = new();

        private void Awake()
        {
            listItemPrefab.SetActive(false);
        }
            
        private void OnEnable()
        {
            if (UserDataModel.Singleton) 
            {
                UserDataModel.Singleton.OnInventoryUpdated += OnReceiveInventoryUpdated;
            }
        }
        private void OnDisable()
        {
            if (UserDataModel.Singleton) 
            {
                UserDataModel.Singleton.OnInventoryUpdated -= OnReceiveInventoryUpdated;
            }
        }

        public override void Show()
        {
            base.Show();

            Initiialize();
        }

        private void Initiialize() 
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();

            foreach (var item in UserDataModel.Singleton.PlayerItemDto.PlayerItems)
            {
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.color = Random.ColorHSV();
                newData.itemName = item.ItemID;
                newData.icon = null; // 맞다 아이콘은 PlayerItemData에는 없지; ItemSO 연결해야하나
                newData.itemCount = item.ItemCount;
                newData.itemPrice = 0; // 인벤토리에선 안쓰니까, 나중에 필요하면 뭐 바꾸지

                infiniteScroll.InsertData(newData);
                infiniteDataContainer.Add(item.ItemID, newData);
            }
        }

        private void OnReceiveInventoryUpdated(PlayerItemDTO.PlayerItemData changedData) 
        {
            bool isExistItem = infiniteDataContainer.ContainsKey(changedData.ItemID);

            if (isExistItem) 
            {
                // TODO : 이미 존재하는 Infinite Data 이므로, 수량만 갱신하여 infinite scroll 에 반영
                infiniteDataContainer[changedData.ItemID].itemCount = changedData.ItemCount;
                infiniteScroll.UpdateData(infiniteDataContainer[changedData.ItemID]);
            }
            else 
            {
                // TODO : 새로운 Infinite Data 생성 후, infinite scroll 에 추가
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.color = Random.ColorHSV();
                newData.itemName = changedData.ItemID;
                newData.icon = null; // 어....? 맞다 아이콘은 PlayerItemData에는 없지; ItemSO 연결해야하나
                newData.itemCount = changedData.ItemCount;
                newData.itemPrice = 0; // 인벤토리에선 안쓰니까, 나중에 필요하면 뭐 바꾸지

                infiniteScroll.InsertData(newData);
                infiniteDataContainer.Add(changedData.ItemID, newData);
            }
        }
    }
}
