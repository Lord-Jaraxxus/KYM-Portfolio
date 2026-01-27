using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class CameraSystem : MonoBehaviour
    {
        public static CameraSystem Instance { get; private set; }
        public bool IsActiveLockOn => lockOnActived;
        public Transform LockOnTarget => lockOnTargetPoint;

        [field: SerializeField] public Vector3 AimingPoint { get; private set; }

        [SerializeField] private Camera mainCamera;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera tpsCamera;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera lockOnCamera;
        private int characterLayerMask;

        private List<CharacterBase> detectedCharacter = new();
        
        private bool lockOnActived = false;
        private Transform lockOnTargetPoint;

        private void Awake() => Instance = this;

        public void Start()
        {
            characterLayerMask = LayerMask.GetMask("Character");
        }

        private void Update()
        {
            Ray screenCenterRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            if (Physics.Raycast(screenCenterRay, out RaycastHit hitInfo, 1000f, characterLayerMask))
            {
                // Player 자기 자신이면 무시
                if (hitInfo.collider.CompareTag("Player"))
                {
                    AimingPoint = screenCenterRay.GetPoint(1000f);
                }
                else
                {
                    AimingPoint = hitInfo.point;
                }
            }
            else
            {
                AimingPoint = screenCenterRay.GetPoint(1000f);
            }

            if (lockOnTargetPoint != null)
            {
                lockOnCamera.Follow.LookAt(lockOnTargetPoint); 
            }
        }

        public void RegisterCharacter(CharacterBase character) 
        {
            detectedCharacter.Remove(character); // 중복 등록 방지
            detectedCharacter.Add(character);
        }

        public void RemoveCharacter(CharacterBase character) 
        {
            detectedCharacter.Remove(character);
        }

        public void SetActiveLockOn(bool isActive)
        {
            lockOnActived = isActive;
            if (lockOnActived)
            {
                lockOnCamera.gameObject.SetActive(true);
                CalculateInsightLockOnPoint(); 
            }
            else
            {
                lockOnCamera.gameObject.SetActive(false);
                lockOnTargetPoint = null;
            }
        }

        public void SetLockOnToggle() => SetActiveLockOn(!lockOnActived);

        private void CalculateInsightLockOnPoint()
        {
            // Pass #1. TODO 카메라의 정면 방향 내에 CharacterBase가 있는지 검사
            List<CharacterBase> inSightCharacters = new();

            Vector3 camDir = mainCamera.transform.forward;
            Vector3 camPos = mainCamera.transform.position;
            foreach (CharacterBase character in detectedCharacter)
            {
                Vector3 direction = (character.transform.position - camPos).normalized;
                float dot = Vector3.Dot(camDir, direction);

                if (dot > 0)
                {
                    inSightCharacters.Add(character);
                }
            }

            // Pass #2. TODO 가장 가까운 CharacterBase를 찾음
            CharacterBase nearestCharacter = null;
            foreach (CharacterBase character in inSightCharacters)
            {
                float distance = Vector3.Distance(camPos, character.transform.position);
                if (nearestCharacter == null || distance < Vector3.Distance(camPos, nearestCharacter.transform.position))
                {
                    nearestCharacter = character;
                }
            }

            // Pass #3. TODO 해당 CharacterBase에서 LockOnPoint에 접근하여, Index값을 넘겨주고, Transform(:Lock on Point)을 받아오기
            if (nearestCharacter != null)
            {
                Transform lockOnPoint = nearestCharacter.GetLockOnPoint(0);
                lockOnTargetPoint = lockOnPoint;
            }
        }
    }
}
