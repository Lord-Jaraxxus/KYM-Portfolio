using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEngine;

namespace KYM
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        public CharacterBase LinkedCharacter => linkedCharacter;

        [field: SerializeField] public Transform CinemachineCameraTarget { get; private set; }

        private CharacterBase linkedCharacter;
        private Camera mainCamera;

        [SerializeField] InteractionSensor sensor;

        [Header("Camera")]
        [SerializeField] private float cameraThreshold = 0.1f; // 카메라 회전 임계값
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        [SerializeField] private float cameraTopClamp = 85.0f;    // 카메라 상단 회전 제한
        [SerializeField] private float cameraBottomClamp = -30.0f;// 카메라 하단 회전 제한
        [SerializeField] private float mouseXSensitivity = 1.0f;
        [SerializeField] private float mouseYSensitivity = 1.0f;

        private CommandInvoker commandInvoker { get; set; }

        private void Awake()
        {
            Instance = this;

            linkedCharacter = GetComponent<CharacterBase>();
            PlayerCharacterContext.Singleton.Register(linkedCharacter); // 플레이어 캐릭터 등록
            mainCamera = Camera.main;

            // 현재 타깃 회전값에서 시작 (튀는 것 방지)
            if (CinemachineCameraTarget != null)
            {
                var e = CinemachineCameraTarget.rotation.eulerAngles;
                cinemachineTargetYaw = e.y;
                cinemachineTargetPitch = e.x;
            }
        }

        private void Start()
        {
            // 캐릭터 초기화
            linkedCharacter.Initialize(GameDataModel.Singleton.PlayerStatDto.playerCharacterStatSO, true);

            // UI - PlayerHUD 초기화
            var playerHUD = UIManager.Singleton.GetUI<PlayerHUD>(UIList.PlayerHUD);
            playerHUD.RefreshHpUI(linkedCharacter.CurHP, linkedCharacter.MaxHP);
            playerHUD.RefreshSpUI(linkedCharacter.CurSP, linkedCharacter.MaxSP);
            playerHUD.RefreshGoldUI(UserDataModel.Singleton.PlayerEconomyDTO.Gold);
            // UI - 상점 UI 초기화
            var shopUI = UIManager.Singleton.GetUI<ShopUI> (UIList.ShopUI);
            // UI - 캐릭터 정보창 UI 초기화
            var characterInfoUI = UIManager.Singleton.GetUI<CharacterInfoUI>(UIList.CharacterInfoUI);

            // 이벤트 구독
            linkedCharacter.OnHpChanged += playerHUD.RefreshHpUI;   // 플레이어 캐릭터 HP 변경시 HUD 갱신
            linkedCharacter.OnSpChanged += playerHUD.RefreshSpUI;   // 플레이어 캐릭터 SP 변경시 HUD 갱신
            UserDataModel.Singleton.OnEconomyUpdated += playerHUD.RefreshGoldUI; // 골드 변경시 HUD 갱신
            shopUI.OnShopClosed += linkedCharacter.SetCharacterState; // 상점 닫기 버튼 클릭시 (상점 닫힐시) 플레이어 상태 변경
            linkedCharacter.OnEquipChanged += characterInfoUI.SetIcon; // 장착 아이템 변경시 장비창의 아이콘 갱신
            linkedCharacter.OnEquipChanged += OnEquipChanged; // 장착 아이템 변경시 콜백 추가

            // Input 이벤트 구독
            InputManager.Singleton.OnInputLmc += OnReceiveInputLmc;
            InputManager.Singleton.onInputTab += OnReceiveInputTab;
            InputManager.Singleton.onInputESC += OnReceiveInputESC;
            InputManager.Singleton.onInputF += OnReceiveInputF;
            InputManager.Singleton.onInputI += OnReceiveInputI;
            InputManager.Singleton.onInputP += OnReceiveInputP;


            // 콤보용 뭐시기들 초기화 예정
            commandInvoker = new CommandInvoker(linkedCharacter.AnimationEventListener);
        }


        private void OnDestroy()
        {
            Instance = null;

            if (linkedCharacter != null) // 연결된 캐릭터가 존재한다면
            {
                var playerHUD = UIManager.Singleton.GetUI<PlayerHUD>(UIList.PlayerHUD);
                var shopUI = UIManager.Singleton.GetUI<ShopUI>(UIList.ShopUI);
                var characterInfoUI = UIManager.Singleton.GetUI<CharacterInfoUI>(UIList.CharacterInfoUI);

                // UI 이벤트 구독 해제
                linkedCharacter.OnHpChanged -= playerHUD.RefreshHpUI;
                linkedCharacter.OnSpChanged -= playerHUD.RefreshSpUI;
                UserDataModel.Singleton.OnEconomyUpdated -= playerHUD.RefreshGoldUI;
                shopUI.OnShopClosed -= linkedCharacter.SetCharacterState;
                linkedCharacter.OnEquipChanged -= characterInfoUI.SetIcon; // 장착 아이템 변경시 장비창의 아이콘 갱신
            }

            // Input 이벤트 구독 해제
            InputManager.Singleton.OnInputLmc   -= OnReceiveInputLmc;
            InputManager.Singleton.onInputF     -= OnReceiveInputF;
            InputManager.Singleton.onInputI     -= OnReceiveInputI;
            InputManager.Singleton.onInputTab   -= OnReceiveInputTab;
            InputManager.Singleton.onInputESC   -= OnReceiveInputESC;
            InputManager.Singleton.onInputP     -= OnReceiveInputP;

            PlayerCharacterContext.Singleton.Unregister(); // 등록된 캐릭터 해제
        }

        private void Update()
        {
            if (linkedCharacter == null) return;

            // 입력
            Vector2 inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool isWalk = Input.GetKey(KeyCode.LeftShift);

            // 이동 기준 전방: 시네머신 타깃의 수평 투영 forward 사용
            Vector3 camForwardFlat = Vector3.ProjectOnPlane(
                CinemachineCameraTarget ? CinemachineCameraTarget.forward : mainCamera.transform.forward,
                Vector3.up
            ).normalized;

            // 상태 전달
            linkedCharacter.IsWalk = isWalk;
            linkedCharacter.SetMovementForward(camForwardFlat);
            linkedCharacter.Move(inputMove);

            if (CameraSystem.Instance.IsActiveLockOn)
            {
                // 캐릭터 방향을 락온 타겟 방향으로 조정, Y값은 바꾸지 않음
                Vector3 direction = (CameraSystem.Instance.AimingPoint - linkedCharacter.transform.position).normalized;
                direction.y = 0f;
                linkedCharacter.transform.forward = direction;

                linkedCharacter.SetStrafe(true);
            }
            else
            {
                linkedCharacter.SetStrafe(false);
            }


            // 공격
            if (Input.GetMouseButtonDown(0))
            {
                linkedCharacter.Attack1();
            }

            // if (commandInvoker.CommandQueue.Count > 0) { commandInvoker.ExecuteNext(); }
        }

        private void LateUpdate()
        {
            if (CameraSystem.Instance.IsActiveLockOn)
            {
                // CinemachineCameraTarget.rotation = Quaternion.identity;
            }
            else
            {
                CameraRotation();
            }
        }

        void OnReceiveInputLmc() => commandInvoker.TryAddCommand(new LeftClickCommand(linkedCharacter));

        private void OnReceiveInputTab()
        {
            CameraSystem.Instance.SetLockOnToggle();
        }

        private void OnReceiveInputESC()
        {
            ShopUI shopUI = UIManager.Singleton.GetUI<ShopUI>(UIList.ShopUI);
            bool isOpen = shopUI != null && shopUI.gameObject.activeSelf;

            if (isOpen && linkedCharacter.CurrentState == CharacterState.Interact) // 상점이 켜져있고, 플레이어가 상호작용 중이라면
            {
                UIManager.Hide<ShopUI>(UIList.ShopUI);
                linkedCharacter.SetCharacterState(CharacterState.Idle); // 이렇게 해도 되남;
                // linkedCharacter.CurrentState = CharacterState.Idle; // 상호작용 상태 해제 -> 이것도 CharacterBase로 옮기고 싶은데, 어떻게?
            }
            else
            {
                // 상점 안켜져있으면인데, 뭐 메뉴라도 띄울까?
                GlobalUI globalUI = UIManager.Show<GlobalUI>(UIList.GlobalUI);
                globalUI.SetMenuPanel(true);
            }
        }

        void OnReceiveInputF()
        {
            if (linkedCharacter.CanInteract() == false) { return; } // 플레이어 캐릭터가 상호작용 불가 상태시 종료

            if (sensor != null && sensor.CurrentTarget != null) // 가까이에 상호작용 가능한 뭔가가 있을 때
            {
                sensor.CurrentTarget.Interact();    // 일단 상호작용 물체의 Interact 메소드 실행
                linkedCharacter.TryInteract(sensor.CurrentTarget.Type); // 상호작용 타입에 따라서 CharacterBase에서 처리
            }
            else // 가까이에 상호작용 할 대상이 없을 때
            {
                Debug.Log("상호작용 대상이 없습니다.");
            }
        }

        void OnReceiveInputI()
        {
            var inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);

            if (inventoryUI.gameObject.activeSelf)
            {
                UIManager.Hide<InventoryUI>(UIList.InventoryUI);
            }
            else
            {
                UIManager.Show<InventoryUI>(UIList.InventoryUI);
            }

            // Debug.Log("인벤토리 토글");
        }
        private void OnReceiveInputP()
        {
            var characterInfoUI = UIManager.Singleton.GetUI<CharacterInfoUI>(UIList.CharacterInfoUI);

            if (characterInfoUI.gameObject.activeSelf)
            {
                UIManager.Hide<CharacterInfoUI>(UIList.CharacterInfoUI);
            }
            else
            {
                UIManager.Show<CharacterInfoUI>(UIList.CharacterInfoUI);
            }
        }

        private void CameraRotation()
        {
            Vector2 inputLook = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (inputLook.sqrMagnitude >= cameraThreshold * cameraThreshold)
            {
                cinemachineTargetYaw += inputLook.x * mouseXSensitivity;
                cinemachineTargetPitch -= inputLook.y * mouseYSensitivity;
            }

            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, cameraBottomClamp, cameraTopClamp);

            if (CinemachineCameraTarget != null)
            {
                CinemachineCameraTarget.rotation = Quaternion.Euler(
                    cinemachineTargetPitch,
                    cinemachineTargetYaw,
                    0.0f
                );
            }
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        // 플레이어 캐릭터의 장착 아이템 변경시 호출되는 콜백
        private void OnEquipChanged(ItemDataSO beforeEquipSO, ItemDataSO newEquipSO) 
        {
            if (newEquipSO != null) // 장비 해제가 아니라면 (새로 장착 or 교체)
            {
                // 같은 슬롯의 장비를 이미 장착하고 있다면, 변수로 가져옴.
                PlayerEquipDTO.PlayerEquipSlotData sameSlotEquip = UserDataModel.Singleton.GetSameSlotEquip(newEquipSO.EquipSlotType);
                // 이미 같은 슬롯의 장비를 장착하고 있다면
                if (sameSlotEquip != null)
                {
                    UserDataModel.Singleton.AddItem(sameSlotEquip.EquippedItemID, 1); // 지금 장착한 장비를 다시 인벤토리로 보냄
                    UserDataModel.Singleton.UpdateEquipedItemData(newEquipSO); // 유저데이터에 장비템 바뀌었다고 갱신하기
                    Debug.Log("장비 교체!");
                }
                else // 해당 슬롯이 비어있었다면
                {
                    UserDataModel.Singleton.UpdateEquipedItemData(newEquipSO);
                    Debug.Log("새 장비!");
                }
            }
            else // 장비 해제라면
            {
                // 선택된 슬롯의 장비를 변수로 가져옴. 없으면 리턴
                PlayerEquipDTO.PlayerEquipSlotData sameSlotEquip = UserDataModel.Singleton.GetSameSlotEquip(beforeEquipSO.EquipSlotType);
                if (sameSlotEquip == null)
                    return;

                UserDataModel.Singleton.AddItem(sameSlotEquip.EquippedItemID, 1); // 지금 장착한 장비를 다시 인벤토리로 보냄
                UserDataModel.Singleton.UneqiupItem(beforeEquipSO); // 그 다음에 UDM의 장비 슬롯 데이터를 갱신(삭제)
            }
        }
    }
}
