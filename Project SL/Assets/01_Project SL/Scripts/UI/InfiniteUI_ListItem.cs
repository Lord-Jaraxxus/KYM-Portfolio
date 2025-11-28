using Gpm.Ui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class InfiniteUI_ListData : Gpm.Ui.InfiniteScrollData 
    {
        public Color color;
        public Sprite icon;
        public string itemName;
        public int itemCount;
    }

    public class InfiniteUI_ListItem : Gpm.Ui.InfiniteScrollItem
    {
        public UnityEngine.UI.Image iconImage;
        public UnityEngine.UI.Image backgroundImage;
        public TMPro.TextMeshProUGUI itemNameText;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            var convertData = scrollData as InfiniteUI_ListData;

            backgroundImage.color = convertData.color;
            iconImage.sprite = convertData.icon;
            itemNameText.text = convertData.itemName;
        }
    }
}
