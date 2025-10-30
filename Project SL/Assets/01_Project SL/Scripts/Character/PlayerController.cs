using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class PlayerController : MonoBehaviour
    {
        [field:SerializeField] public Transform CinemachineCameraTarget { get; private set; }

        private CharacterBase linkedCharacter;
        private Camera mainCamera;

        private float cameraTrheshold = 0.1f; // ī�޶� ȸ�� �Ӱ谪
        private float chinemachineTargetYaw;
        private float cinemachineTargetPitch;
        private float cameraTopClamp = 85.0f; // ī�޶� ��� ȸ�� ����
        private float cameraBottomClamp = -30.0f; // ī�޶� �ϴ� ȸ�� ����

        private CommandInvoker commandInvoker { get; set; }

        private void Awake()
        {
            linkedCharacter = GetComponent<CharacterBase>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            InputManager.Singleton.OnInputLmc += OnReceiveInputLmc;

            commandInvoker = new CommandInvoker(linkedCharacter.AnimationEventListener);
        }

        private void Update()
        {
            if(linkedCharacter == null) return;

            Vector2 inputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            linkedCharacter.IsWalk = Input.GetKey(KeyCode.LeftShift);
            linkedCharacter.SetMovementForward(mainCamera.transform.forward);
            linkedCharacter.Move(inputMove);
            linkedCharacter.Rotate(CameraSystem.Instance.AimingPoint);

            if (commandInvoker.CommandQueue.Count > 0) { commandInvoker.ExecuteNext(); }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        void OnReceiveInputLmc() => commandInvoker.TryAddCommand(new LeftClickCommand(linkedCharacter));



        private void CameraRotation() 
        {
            Vector2 inputLook = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (inputLook.sqrMagnitude >= cameraTrheshold) 
            {
                float yaw = inputLook.x;
                float pitch = inputLook.y;

                chinemachineTargetYaw += yaw;
                cinemachineTargetPitch -= pitch;
            }

            chinemachineTargetYaw = ClampAngle(chinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, cameraBottomClamp, cameraTopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, chinemachineTargetYaw, 0.0f);
        }
    
        private float ClampAngle(float angle, float min, float max) 
        {
            if(angle < -360f) angle += 360f;
            if(angle > 360f) angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }
    }
}
