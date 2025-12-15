using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class AISensor : MonoBehaviour
    {
        public CharacterBase DetectedTarget => detectedTarget;

        [Header("Sensor Settings")]
        [SerializeField] private float sensorRadius = 5f; // 센서의 반지름

        [SerializeField] private Rigidbody sensorRigidbody;
        [SerializeField] private SphereCollider sensorCollider;

        public System.Action<CharacterBase> OnDetectedCharacterEvent; // 감지된 캐릭터에 대한 이벤트
        public System.Action<CharacterBase> OnLostCharacterEvent; // 감지된 캐릭터가 사라졌을 때의 이벤트

        private CharacterBase detectedTarget;

        private void Awake()
        {
            if (false == TryGetComponent(out sensorRigidbody)) 
            {
                sensorRigidbody = gameObject.AddComponent<Rigidbody>();
                sensorRigidbody.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
            }

            if (false == TryGetComponent(out sensorCollider)) 
            {
                sensorCollider = gameObject.AddComponent<SphereCollider>();
                sensorCollider.isTrigger = true; // 센서 콜라이더를 트리거로 설정
            }
        }

        private void Start()
        {
            SetSensorRadius(sensorRadius);
        }

        private void SetSensorRadius(float radius) 
        {
            sensorCollider.radius = radius; // 센서의 반지름 설정
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character)) 
            {
                detectedTarget = character;
                OnDetectedCharacterEvent?.Invoke(character); // 캐릭터가 감지되면 이벤트 호출
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                detectedTarget = null;
                OnLostCharacterEvent?.Invoke(character); // 캐릭터가 감지 영역을 벗어나면 이벤트 호출
            }
        }
    }
}
