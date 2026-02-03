using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class CharacterEquipUI : UIBase
    {
        public override bool IsNeedCursorVisible => true;

        // 콘텍스트 팝업 관련
        [SerializeField] private Button modalButton;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button unequipButton;
        [SerializeField] private Button dropButton;
        private ItemDataSO selectedEquipData = null;

        // 장비 슬롯 버튼들
        [SerializeField] private Button button_Head;
        [SerializeField] private Button button_Body;
        [SerializeField] private Button button_Legs;
        [SerializeField] private Button button_Weapon;
        [SerializeField] private Button button_Shield;

        // 장비 슬롯에 들어갈 아이콘들
        [SerializeField] private Image icon_Head;
        [SerializeField] private Image icon_Body;
        [SerializeField] private Image icon_Legs;
        [SerializeField] private Image icon_Weapon;
        [SerializeField] private Image icon_Shield;



        private void Awake()
        {
            modalButton.onClick.AddListener(OnClickModalButton);
            unequipButton.onClick.AddListener(OnClickUnequipButton);
            dropButton.onClick.AddListener(OnClickDropButton);

            button_Head.onClick.AddListener(OnClickSlotButton_Head);
            button_Body.onClick.AddListener(OnClickSlotButton_Body);
            button_Legs.onClick.AddListener(OnClickSlotButton_Legs);
            button_Weapon.onClick.AddListener(OnClickSlotButton_Weapon);
            button_Shield.onClick.AddListener(OnClickSlotButton_Shield);
        }

        private void OnEnable()
        {
            panel.SetActive(false); // 버튼 패널 꺼두기
            modalButton.gameObject.SetActive(false); // 모달도 꺼두기

            PlayerController.Instance.LinkedCharacter.OnEquipChanged += SetIcon; // 장비 변경시 아이콘 갱신되도록 이벤트 연결
            Initialize(); // 초기 세팅
        }

        private void OnDisable()
        {
            PlayerController.Instance.LinkedCharacter.OnEquipChanged -= SetIcon; // 이벤트 해제
        }

        private void OnClickSlotButton(EquipSlotType type) => OnClickEquipSlotButton(type);
        private void OnClickSlotButton_Head() => OnClickEquipSlotButton(EquipSlotType.Head);
        private void OnClickSlotButton_Body() => OnClickEquipSlotButton(EquipSlotType.Body);
        private void OnClickSlotButton_Legs() => OnClickEquipSlotButton(EquipSlotType.Legs);
        private void OnClickSlotButton_Weapon() => OnClickEquipSlotButton(EquipSlotType.Weapon);
        private void OnClickSlotButton_Shield() => OnClickEquipSlotButton(EquipSlotType.Shield);

        private void Initialize() 
        {
            // 초기 아이콘 세팅
            foreach(var slotData in UserDataModel.Singleton.PlayerEquipDto.PlayerEquipSlots) 
            {
                if (slotData.EquipedItemDataSO != null) 
                {
                    SetIcon(null, slotData.EquipedItemDataSO);
                }
            }
        }

        public void SetIcon(ItemDataSO beforeEquipSO, ItemDataSO newEquipSO)
        {
            if (newEquipSO != null) // 장비 해제가 아니라면 (장착, 교체)
            {
                switch (newEquipSO.EquipSlotType)
                {
                    case EquipSlotType.Head:
                        icon_Head.sprite = newEquipSO.Icon;
                        icon_Head.enabled = true;
                        break;
                    case EquipSlotType.Body:
                        icon_Body.sprite = newEquipSO.Icon;
                        icon_Body.enabled = true;
                        break;
                    case EquipSlotType.Legs:
                        icon_Legs.sprite = newEquipSO.Icon;
                        icon_Legs.enabled = true;
                        break;
                    case EquipSlotType.Weapon:
                        icon_Weapon.sprite = newEquipSO.Icon;
                        icon_Weapon.enabled = true;
                        break;
                    case EquipSlotType.Shield:
                        icon_Shield.sprite = newEquipSO.Icon;
                        icon_Shield.enabled = true;
                        break;
                    default:
                        break;
                }
            }
            else // 장비 해제라면
            {
                switch (beforeEquipSO.EquipSlotType)
                {
                    case EquipSlotType.Head:
                        icon_Head.enabled = false;
                        break;
                    case EquipSlotType.Body:
                        icon_Body.enabled = false;
                        break;
                    case EquipSlotType.Legs:
                        icon_Legs.enabled = false;
                        break;
                    case EquipSlotType.Weapon:
                        icon_Weapon.enabled = false;
                        break;
                    case EquipSlotType.Shield:
                        icon_Shield.enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetPopupActive(bool isActive)
        {
            panel.SetActive(isActive);
            modalButton.gameObject.SetActive(isActive);
        }

        private void OnClickEquipSlotButton(EquipSlotType slotType)
        {
            PlayerEquipDto.PlayerEquipSlotData sameSlotEquip = UserDataModel.Singleton.GetSameSlotEquip(slotType);
            if (sameSlotEquip != null) // 해당 슬롯에 장비가 있을 경우에만
            {
                selectedEquipData = sameSlotEquip.EquipedItemDataSO;

                SetPopupActive(true);
            }

            // 팝업 위치 조정 (마우스 근처로 뜨게)
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
        }

        private void OnClickModalButton()
        {
            SetPopupActive(false);
        }

        private void OnClickUnequipButton()
        {
            // TODO : 장비를 해제하고 인벤토리에 돌려놓기.
            CharacterBase playerCharacter = PlayerCharacterContext.Singleton.CurrentPlayerCharacter; // 현재 플레이어 캐릭터 가져옴
            playerCharacter.UneqipItem(selectedEquipData);

            SetPopupActive(false);
        }

        private void OnClickDropButton()
        {
            SetPopupActive(false);
        }

    }
}
