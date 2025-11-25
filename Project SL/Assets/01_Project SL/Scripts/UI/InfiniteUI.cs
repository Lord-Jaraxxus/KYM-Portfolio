using System.Collections;
using System.Collections.Generic;
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
            for (int i=0; i<100; i++)
            {
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.color = Random.ColorHSV();
                newData.itemName = $"Item {i}";
                newData.icon = null; // Assign an icon if available

                infiniteScroll.InsertData(newData);
            }
        }
    }
}
