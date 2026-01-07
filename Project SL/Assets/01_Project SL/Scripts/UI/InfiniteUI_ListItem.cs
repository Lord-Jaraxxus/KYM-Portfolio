using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class InfiniteUI_ListData : Gpm.Ui.InfiniteScrollData
    {
        public Color color;
        public Sprite icon;
        public string itemID;
        public string itemName;
        public int itemCount;
        public int itemPrice;
    }

    public class InfiniteUI_ListItem : Gpm.Ui.InfiniteScrollItem
    {
        public UnityEngine.UI.Image iconImage;
        public UnityEngine.UI.Image backgroundImage;
        public TMPro.TextMeshProUGUI itemNameText;
        public TMPro.TextMeshProUGUI itemCountText;
        public TMPro.TextMeshProUGUI itemPriceText;

        public string itemID;
        public UnityEngine.UI.Button itemButton;
        public Action<InfiniteUI_ListData> onItemClicked;
        InfiniteUI_ListData listData;

        private void Start()
        {
            // 아이템 버튼 클릭 이벤트 연결 초기화하고 연결, 근데 이거 Start에 있어도 되나? 잘 돌아가긴 하던데.. 밑에 UpdateData에 있는 게 정석인가?
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnClickItemButton);

            if (itemPriceText != null) // (인벤토리 or 상점 구분) 아이템 가격에 대한 GUI가 있다면, 즉 상점에 표시된 아이템이라면. 일단 이렇게 처리해놓긴 했는데;  
            {
                // 아이템 버튼 클릭 이벤트를 상점 UI의 메서드에 구독시켜줌
                ShopUI shop = GetComponentInParent<ShopUI>();
                onItemClicked += shop.OnClickPurchaseButtonFromList;
            }
            else // 인벤토리에서 열렸다면.
            {
                InventoryUI inventory = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
                onItemClicked += inventory.OnClickItemButton;
            }
        }

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            listData = scrollData as InfiniteUI_ListData;
            itemID = listData.itemID;

            backgroundImage.color = listData.color;
            iconImage.sprite = listData.icon;
            itemNameText.text = listData.itemName;
            itemCountText.text = $"x {listData.itemCount}";

            if (itemPriceText != null) // itemPriceText가 있다면 (상점이라면). 인벤토리엔 가격표시가 없으니까 따로 빼둠
            {
                itemPriceText.text = $"{listData.itemPrice}G";

                if (listData.itemCount <= 0) // 아이템이 0개 이하라면. 
                {
                    itemButton.interactable = false; // 구매 버튼 비활성화
                }
                else if (listData.itemPrice > UserDataModel.Singleton.PlayerEconomyDto.Gold) // 플레이어가 가진 골드보다 아이템 가격이 더 비싸다면
                {
                    itemButton.interactable = false; // 구매 버튼 비활성화
                }
                else // 둘 다 아니라면
                {
                    itemButton.interactable = true; // 구매 버튼 활성화
                }
            }  
        }

        private void OnClickItemButton()
        {
            onItemClicked?.Invoke(listData);
        }
    }
}
