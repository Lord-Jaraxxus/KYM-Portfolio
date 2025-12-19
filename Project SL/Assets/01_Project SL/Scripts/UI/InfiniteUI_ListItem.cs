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
            if (itemPriceText != null) // (인벤토리 or 상점 구분) 아이템 가격에 대한 GUI가 있다면, 즉 상점에 표시된 아이템이라면. 일단 이렇게 처리해놓긴 했는데;  
            {
                // 버튼 연결

                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(OnClickItemButton);

                ShopUI shop = GetComponentInParent<ShopUI>();
                onItemClicked += shop.OnClickPurchaseButtonFromList;
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

            if (itemPriceText != null) { itemPriceText.text = $"{listData.itemPrice}G"; }  // itemPriceText가 없다면 ㄴㄴ. 인벤토리엔 가격표시가 없으니까 따로 빼둠
        }

        private void OnClickItemButton()
        {
            onItemClicked?.Invoke(listData);
        }
    }
}
