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

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            listData = scrollData as InfiniteUI_ListData;
            itemID = listData.itemID;

            backgroundImage.color = listData.color;
            iconImage.sprite = listData.icon;
            itemNameText.text = listData.itemName;
            itemCountText.text = $"x {listData.itemCount}";
            itemPriceText.text = $"{listData.itemPrice}G";
        }

        // 버튼 연결용
        private void Start()
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnClickItemButton);

            ShopUI shop = GetComponentInParent<ShopUI>();
            onItemClicked += shop.OnClickPurchaseButtonFromList;
        }

        private void OnClickItemButton()
        {
            onItemClicked?.Invoke(listData);
        }
    }
}
