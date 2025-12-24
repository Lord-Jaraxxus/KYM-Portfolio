using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace KYM
{
    public class InventoryUI : UIBase
    {
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

        private void Initiialize()
        {
            infiniteScroll.ClearData();
            infiniteDataContainer.Clear();

            foreach (PlayerItemDTO.PlayerItemData item in UserDataModel.Singleton.PlayerItemDto.PlayerItems)
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
        }

        private void OnReceiveInventoryUpdated(PlayerItemDTO.PlayerItemData changedData)
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
                // TODO : 새로운 Infinite Data 생성 후, infinite scroll 에 추가
                InfiniteUI_ListData newData = new InfiniteUI_ListData();
                newData.itemID = changedData.ItemID;
                newData.color = Random.ColorHSV();
                newData.itemName = changedData.ItemID;
                newData.icon = null; // 어....? 맞다 아이콘은 PlayerItemData에는 없지; ItemSO 연결해야하나
                newData.itemCount = changedData.ItemCount;
                newData.itemPrice = 0; // 인벤토리에선 안쓰니까, 나중에 필요하면 뭐 바꾸지

                infiniteScroll.InsertData(newData);
                infiniteDataContainer.Add(changedData.ItemID, newData);
            }
        }

        // 아이템 버튼을 눌렀을 때, InfiniteUI_ListItem에서 이벤트가 날아오면 실행
        public void OnClickItemButton(InfiniteUI_ListData data)
        {
            panel.SetActive(true); // 팝업 패널 활성화
            modalButton.gameObject.SetActive(true); // 모달도 같이 활성화

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
            panel.SetActive(false); // 팝업 패널 비활성화
            modalButton.gameObject.SetActive(false); // 모달도 같이 비활성화

            selectedItemData = null;
        }

        private void OnClickUseButton()
        {
            panel.SetActive(false); // 팝업 패널 비활성화
            modalButton.gameObject.SetActive(false); // 모달도 같이 비활성화

            // TODO : 아이템 사용, 즉 아이템 갯수를 1개 줄이고 0개가 되면 인벤토리에서 없애기 + 장비면 장착, 소모품이면 효과 적용 해야함...;

            // 받아온 PlayerItemData에서 ID를 가져와 Itembase에서 ID로 검색해서 해당 아이템의 itemDataSO를 가져옴, itemDataSO변수에 담김, 해당 아이템의 SO가 없으면 리턴
            if (!GameDataModel.Singleton.ItemDatabase.ItemDatas.TryGetValue(selectedItemData.itemID, out ItemDataSO itemDataSO))
                return;

            switch (itemDataSO.ItemCategory)
            {
                case ItemCategory.Equipment:
                    UserDataModel.Singleton.RemoveItem(selectedItemData.itemID, 1); // 일단 장비템도 사용..하는걸로.
                    break;
                case ItemCategory.Consumable:
                    UserDataModel.Singleton.RemoveItem(selectedItemData.itemID, 1);
                    break;
                default:
                    break;
            }
        }

        private void OnClickDropButton()
        {
            panel.SetActive(false); // 팝업 패널 비활성화
            modalButton.gameObject.SetActive(false); // 모달도 같이 비활성화

            // TODO : 이건 진짜 빡센데? 모든 아이템 프리펩 들고있을 수도 없고 ㅋㅋㅋㅋ
        }

    }
}
