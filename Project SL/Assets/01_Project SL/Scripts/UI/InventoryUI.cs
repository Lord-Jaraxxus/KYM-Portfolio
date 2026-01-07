using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace KYM
{
    public class InventoryUI : UIBase
    {
        // 콘텍스트 팝업 관련
        [SerializeField] private Button modalButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button useButton;
        [SerializeField] private Button dropButton;

        // Infinite UI 관련
        public Gpm.Ui.InfiniteScroll infiniteScroll;
        public GameObject listItemPrefab;
        private Dictionary<string, InfiniteUI_ListData> infiniteDataContainer = new();

        // 클릭된 아이템 데이터
        private InfiniteUI_ListData selectedItemData = null;

        private void Awake()
        {
            listItemPrefab.SetActive(false);

            // 버튼 연결
            modalButton.onClick.AddListener(OnclickModalButton);
            useButton.onClick.AddListener(OnClickUseButton);
            dropButton.onClick.AddListener(OnClickDropButton);
        }

        private void OnEnable()
        {
            panel.SetActive(false); // 버튼 패널 꺼두기
            modalButton.gameObject.SetActive(false); // 모달도 꺼두기

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

        private void AddInventoryItem(PlayerItemDto.PlayerItemData item)
        {
            // 받아온 PlayerItemData에서 ID를 가져와 Itembase에서 ID로 검색해서 해당 아이템의 itemDataSO를 가져옴, itemDataSO변수에 담김, 해당 아이템의 SO가 없으면 리턴
            if (!GameDataModel.Singleton.ItemDatabase.ItemDatas.TryGetValue(item.ItemID, out ItemDataSO itemDataSO))
                return;

            InfiniteUI_ListData newData = new InfiniteUI_ListData();
            newData.itemID = itemDataSO.ItemID;
            newData.color = Random.ColorHSV(); // 색은.. 아직 ㅎ; 굳이인가 싶기도 하고
            newData.itemName = itemDataSO.ItemName;
            newData.icon = itemDataSO.Icon;
            newData.itemPrice = itemDataSO.Price;

            // 얘는 따로 관리해야 하니까 PlayerItems에서 정보를 가져옴
            newData.itemCount = item.ItemCount;

            infiniteScroll.InsertData(newData);
            infiniteDataContainer.Add(item.ItemID, newData);
        }

        private void Initiialize()
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();

            foreach (PlayerItemDto.PlayerItemData item in UserDataModel.Singleton.PlayerItemDto.PlayerItems)
            {
                AddInventoryItem(item);
            }
        }

        private void OnReceiveInventoryUpdated(PlayerItemDto.PlayerItemData changedData)
        {
            bool isExistItem = infiniteDataContainer.ContainsKey(changedData.ItemID);

            if (isExistItem) // 이미 인벤토리에 있었던 템인 경우
            {
                // TODO : 아이템 수량이 0이 되었으면 infiniteDataContainer와 infiniteScroll에서 제거
                if (changedData.ItemCount <= 0)
                {
                    InfiniteUI_ListData listData = infiniteDataContainer[changedData.ItemID];
                    infiniteScroll.RemoveData(listData);
                    infiniteDataContainer.Remove(changedData.ItemID);
                }
                else
                {
                    // 이미 존재하는 Infinite Data 이므로, 수량만 갱신하여 infinite scroll 에 반영
                    infiniteDataContainer[changedData.ItemID].itemCount = changedData.ItemCount;
                    infiniteScroll.UpdateData(infiniteDataContainer[changedData.ItemID]);
                }
            }
            else // 새로 인벤토리에 추가된 템인 경우
            {
                AddInventoryItem(changedData);
            }
        }


        private void SetPopupActive(bool isActive)
        {
            panel.SetActive(isActive);
            modalButton.gameObject.SetActive(isActive);
        }

        // 아이템 버튼을 눌렀을 때, InfiniteUI_ListItem에서 이벤트가 날아오면 실행
        public void OnClickItemButton(InfiniteUI_ListData data)
        {
            SetPopupActive(true); // 팝업 활성화

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);     //Screen 좌표계
            mousePos.z = 0;

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            RectTransform canvasRect = panel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null, // Screen Space - Overlay니까 null
                out localPos
            );

            panelRect.anchoredPosition = localPos;

            selectedItemData = data; // 현재 선택한 아이템의 데이터를 저장
        }

        private void OnclickModalButton()
        {
            SetPopupActive(false); // 팝업 비활성화

            selectedItemData = null;
        }

        private void OnClickUseButton()
        {
            SetPopupActive(false); // 팝업 비활성화

            ItemSystem.UseItem(selectedItemData.itemID);
        }

        private void OnClickDropButton()
        {
            SetPopupActive(false); // 팝업 비활성화

            ItemSystem.DropItem(selectedItemData.itemID); // 일단 통으로 다 버리기로...
        }

    }
}
