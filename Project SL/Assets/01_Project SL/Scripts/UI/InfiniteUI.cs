using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KYM
{
    public class InfiniteUI : UIBase
    {
        public Gpm.Ui.InfiniteScroll infiniteScroll;
        public GameObject listItemPrefab;

        private void Awake()
        {
            listItemPrefab.SetActive(false);
        }
            
            
        private void Start()
        {
            UserDataModel.Singleton.OnInventoryUpdated += OnReceiveInventoryUpdated;
        }

        private void OnReceiveInventoryUpdated() // 일단 다 날려버리고 처음부터 새로 넣는데, 더 좋은 방법이 아마?
        {
            infiniteScroll.ClearData();

            foreach (var item in UserDataModel.Singleton.PlayerItemDto.PlayerItems) 
            {
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.color = Random.ColorHSV();
                newData.itemName = item.ItemID;
                newData.icon = null; // Assign an icon if available

                infiniteScroll.InsertData(newData);
            }
        }
    }
}
