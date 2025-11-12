using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class PlayerController : MonoBehaviour
    {
        [field: SerializeField] public Transform CinemachineCameraTarget { get; private set; }

        private CharacterBase linkedCharacter;
        private Camera mainCamera;

        [SerializeField] DropItemSensor sensor;

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
            linkedCharacter = GetComponent<CharacterBase>();
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
            SoundManager.PlayBGM("BGM_Garden");

            linkedCharacter.Initialize(GameDataModel.Singleton.PlayerStatDto.playerCharacterStatSO, true);

            InputManager.Singleton.OnInputLmc += OnReceiveInputLmc;
            InputManager.Singleton.onInputF += OnReceiveInputF;

            commandInvoker = new CommandInvoker(linkedCharacter.AnimationEventListener);
        }

        private void Update()
        {
            if (linkedCharacter == null) return;

            // 입력
            Vector2 inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool isWalk = Input.GetKey(KeyCode.LeftShift);
            bool isAim = Input.GetMouseButton(1); // 우클릭 시 정조준(Strafe) 예시

            // 이동 기준 전방: 시네머신 타깃의 수평 투영 forward 사용
            Vector3 camForwardFlat = Vector3.ProjectOnPlane(
                CinemachineCameraTarget ? CinemachineCameraTarget.forward : mainCamera.transform.forward,
                Vector3.up
            ).normalized;

            // 상태 전달
            linkedCharacter.IsWalk = isWalk;
            linkedCharacter.SetMovementForward(camForwardFlat);
            linkedCharacter.SetStrafe(isAim);
            linkedCharacter.Move(inputMove);

            // 공격
            if (Input.GetMouseButtonDown(0))
            {
                linkedCharacter.Attack1();
            }

            // if (commandInvoker.CommandQueue.Count > 0) { commandInvoker.ExecuteNext(); }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        void OnReceiveInputLmc() => commandInvoker.TryAddCommand(new LeftClickCommand(linkedCharacter));
        void OnReceiveInputF() 
        {
 
               if(sensor.CurrentTarget == null)
               {
                      Debug.Log("커런트 타겟이 없습니다.");
               }
            

            if (sensor != null && sensor.CurrentTarget != null)
            {
                sensor.CurrentTarget.Interact();
            }
            else
            {
                Debug.Log("획득 가능한 아이템이 없습니다.");
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
    }
}
