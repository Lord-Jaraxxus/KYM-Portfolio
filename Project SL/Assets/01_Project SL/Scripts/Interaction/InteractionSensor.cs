using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class InteractionSensor : MonoBehaviour
    {
        private List<IInteractable> detectedInteractables = new List<IInteractable>();
        private IInteractable currentTarget;
        [SerializeField] private Collider sensorCollider;

        public IInteractable CurrentTarget => currentTarget;

        private void Awake()
        {
            sensorCollider = GetComponent<Collider>();
            sensorCollider.isTrigger = true;    // 센서는 반드시 trigger여야 함
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();

            if (interactable != null && !detectedInteractables.Contains(interactable))
            {
                detectedInteractables.Add(interactable);
                // Debug.Log($"Item detected: {interactable}");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null && detectedInteractables.Contains(interactable))
            {
                detectedInteractables.Remove(interactable);
                if (currentTarget == interactable)
                {
                    currentTarget = null;
                }
            }
        }

        private void Update()
        {
            UpdateNearstTarget();
        }

        private void UpdateNearstTarget()
        {
            IInteractable nearest = null;
            float minDistance = float.MaxValue;

            foreach (var interactable in detectedInteractables)
            {
                if (interactable == null)
                {
                    detectedInteractables.Remove(interactable); 
                }
            }

            // 감지된 리스트에서 가장 가까운 interactable 찾기
            foreach (var interactable in detectedInteractables)
            {
                var mb = interactable as MonoBehaviour;
                if (mb == null) continue;  // 인터페이스지만 실제 컴포넌트가 아닐 경우 대비

                float distance = Vector3.Distance(transform.position, mb.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = interactable;
                }
            }

            // 가장 가까운 interactable이 바뀌었을 때만 갱신
            if (currentTarget != nearest)
            {
                currentTarget = nearest;

                // 필요하면 여기서 "타겟이 바뀜" 이벤트 처리 가능
                // ex) UIManager.ShowInteractionIcon(currentTarget);
                // Debug.Log($"Target changed: {nearest}");
            }
        }
    }
}
