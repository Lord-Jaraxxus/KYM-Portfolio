using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class ShopUI : UIBase
    {
        [SerializeField] public int shopID;    // 얘가 여기 있어도 되남;
        ShopDataSO shopDataSO;          // 상점 데이터 (품목, 수량, 가격)
        bool initialized = false;

        // 상점 카테고리 토글 관련
        ItemCategory Currentcategory;
        public ToggleGroup toggleGroup;        // 상점 카테고리 토글 그룹
        public List<Toggle> toggles;           // 상점 카테고리 토글들

        // 인피니티 스크롤 관련 
        public Gpm.Ui.InfiniteScroll infiniteScroll;
        public GameObject listItemPrefab;
        private Dictionary<string, InfiniteUI_ListData> infiniteDataContainer = new();
        public Dictionary<string, int> itemStocks = new();

        private void Awake()
        {
            listItemPrefab.SetActive(false);
        }

        private void Start()
        {
            toggles = new List<Toggle>(toggleGroup.GetComponentsInChildren<Toggle>());
            for (int i = 0; i < toggles.Count; i++) // 토글들에게 이벤트 리스너 등록
            {
                int index = i; // 캡쳐 문제 방지용
                toggles[i].onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        OnToggleSelected(index);
                    }
                });
            }
        }

        public override void Show()
        {
            base.Show();

            if (!initialized)
            {
                Initiialize();
                initialized = true;
            }
            else 
            { 
                RefreshShopList(); 
            }
                
        }

        private void Initiialize()  // 초기화 함수, 상점UI 맨 처음 부를때만 호출되도록
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();
            // 토글도 맨 앞엣놈으로 초기화 시켜야대는데

            foreach (ShopDataSO so in GameDataModel.Singleton.ShopDataDTO.ShopDatas) // 상점 ID에 맞는 상점 데이터 불러오기
            {
                if (so.shopID == shopID)
                {
                    shopDataSO = so;
                    break;
                }
            }


            foreach (var item in shopDataSO.itemsForSale)
            {
                itemStocks.Add(item.ID, item.ItemCount); // 재고 수량 따로 저장

                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.itemID = item.ID;
                newData.icon = item.icon;
                newData.itemName = item.itemName;
                newData.itemCount = item.ItemCount;
                newData.itemPrice = item.price;
                newData.color = Color.gray; // Default color for shop items

                infiniteScroll.InsertData(newData); 
                infiniteDataContainer.Add(item.ID, newData);
            }
        }

        public void OnClickPurchaseButtonFromList(InfiniteUI_ListData data) 
        {
            // TODO : 구매 버튼 클릭 시 처리 로직 추가 필요
            Debug.Log(data.itemID);
            if (data == null) return;

            bool isExistItem = infiniteDataContainer.ContainsKey(data.itemID);

            if (isExistItem)
            {
                // TODO : 이미 존재하는 Infinite Data 이므로, 수량만 갱신하여 infinite scroll 에 반영

                if (itemStocks[data.itemID] <= 0) { return; } // 아이템 수량이 0 이하면 구매 불가
                itemStocks[data.itemID] -= 1; // 재고 수량 감소   

                infiniteDataContainer[data.itemID].itemCount = itemStocks[data.itemID];
                infiniteScroll.UpdateData(infiniteDataContainer[data.itemID]); 
            }
            else
            {
                Debug.LogError("구매한 아이템이 존재하지 않습니다. 아이템 이름: " + data.itemName);
            }

            // TODO : 수량 0 이하로 내려갔을때 처리 필요 (상점에서 제거 등)
        }

        private void RefreshShopList()
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();

            foreach (var item in shopDataSO.itemsForSale)
            {
                if (item.category != Currentcategory && Currentcategory != ItemCategory.All)
                {
                    continue;// 현재 카테고리에 해당하지 않는 아이템은 건너뜀
                }

                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.itemID = item.ID;
                newData.icon = item.icon;
                newData.itemName = item.itemName;
                newData.itemPrice = item.price;
                newData.color = Color.gray; // Default color for shop items
             
                newData.itemCount = itemStocks[item.ID]; // 따로 저장한 재고 수량 사용

                infiniteScroll.InsertData(newData);
                infiniteDataContainer.Add(item.ID, newData);
            }
        }


        private void OnToggleSelected(int index)
        {
            Currentcategory = (ItemCategory)index;

            RefreshShopList();
        }
    }
}
