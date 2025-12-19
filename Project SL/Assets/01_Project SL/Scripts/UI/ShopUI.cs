using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class ShopUI : UIBase
    {
        [SerializeField] public string shopID;    // 얘가 여기 있어도 되남;
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

        // 여기에 들고있는게 편하겠다, 유저데이터 출신임
        PlayerShopDTO.ShopData shopData; 

        private void Awake()
        {
            listItemPrefab.SetActive(false);
        }

        private void Start()
        {
            toggles = new List<Toggle>(toggleGroup.GetComponentsInChildren<Toggle>());
            for (int i = 0; i < toggles.Count; i++) // 토글들에게 이벤트 리스너 등록
            {
                int index = i; // 캡쳐 문제 방지용(?)
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

        private void Initiialize()  // 초기화 함수, 상점UI 맨 처음 부를때만 호출되도록 -> 근데 상점 여러개 만들면 어떡함?;;
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();
            // 토글도 맨 앞엣놈으로 초기화 시켜야대는데

            foreach (ShopDataSO so in GameDataModel.Singleton.ShopDataDTO.ShopDatas) // 상점 ID에 맞는 상점 데이터 불러오기
            {
                if (so.ShopID.Equals(shopID))   // SO의 ShopID가 이 UI의 shopID와 같다면
                {
                    shopDataSO = so;
                    break;
                }
            }

            // 게임 데이터 모델에서 상점 재고 데이터 불러오기 
            foreach (var item in shopDataSO.ItemsForSale)
            {
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.itemID = item.ItemID;
                newData.icon = item.Icon;
                newData.itemName = item.ItemName;
                newData.itemCount = item.ItemCount; // TODO : 나중에 UserDataModel에서 재고 수량 불러오도록 변경 필요 (세이브파일에서)
                newData.itemPrice = item.Price;
                newData.color = Color.gray; // Default color for shop items

                infiniteScroll.InsertData(newData); 
                infiniteDataContainer.Add(item.ItemID, newData);
            }

            shopData = UserDataModel.Singleton.PlayerShopDTO.ShopDatas.Find(shop => shop.ShopID == shopID); // 유저 데이터에서 이 상점의 재고 데이터 찾아서 연결
        }

        public void OnClickPurchaseButtonFromList(InfiniteUI_ListData data) 
        {
            // TODO : 구매 버튼 클릭 시 처리 로직
            Debug.Log(data.itemID);
            if (data == null) return;

            bool isExistItem = infiniteDataContainer.ContainsKey(data.itemID);

            if (isExistItem)
            {
                // TODO : 이미 존재하는 Infinite Data 이므로, 수량만 갱신하여 infinite scroll 에 반영
                PlayerShopDTO.ItemStock itemStock = shopData.ItemStocks.Find(shopData => shopData.ItemID == data.itemID);
                if(itemStock.ItemCount <= 0) { return; } // 아이템 재고 수량이 0 이하면 구매 불가
                if (UserDataModel.Singleton.PlayerEconomyDTO.Gold < data.itemPrice) { return; } // 소지 골드가 부족하면 구매 불가

                itemStock.DecreaseStock(1); // 아이템 재고 수량 감소

                // 인피니티 스크롤 데이터 갱신
                infiniteDataContainer[data.itemID].itemCount = itemStock.ItemCount;
                infiniteScroll.UpdateData(infiniteDataContainer[data.itemID]);

                // UserDataModel 안의 Data 갱신
                UserDataModel.Singleton.AddItem(data.itemID, 1); // 플레이어 인벤토리에 아이템 1개 추가, 이거 작동하나??? 잘 되네?
                UserDataModel.Singleton.SubtractGold(data.itemPrice); // 플레이어 소지 골드에서 아이템 가격만큼 차감 처리
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

            foreach (var item in shopDataSO.ItemsForSale)
            {
                if (item.Category != Currentcategory && Currentcategory != ItemCategory.All)
                {
                    continue;// 현재 카테고리에 해당하지 않는 아이템은 건너뜀
                }

                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.itemID = item.ItemID;
                newData.icon = item.Icon;
                newData.itemName = item.ItemName;
                newData.itemPrice = item.Price;
                newData.color = Color.gray; // Default color for shop items

                PlayerShopDTO.ItemStock itemStock = shopData.ItemStocks.Find(shopData => shopData.ItemID == item.ItemID);
                newData.itemCount = itemStock.ItemCount; // 재고 수량은 유저 데이터에서 따로 관리중인걸 가져와서 반영

                infiniteScroll.InsertData(newData);
                infiniteDataContainer.Add(item.ItemID, newData);
            }
        }


        private void OnToggleSelected(int index)
        {
            Currentcategory = (ItemCategory)index;

            RefreshShopList();
        }
    }
}
