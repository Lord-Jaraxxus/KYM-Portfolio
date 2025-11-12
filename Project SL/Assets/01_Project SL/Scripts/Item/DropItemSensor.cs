using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class DropItemSensor : MonoBehaviour
    {
        private List<DropItem> detectedItems = new List<DropItem>();
        private DropItem currentTarget;
        [SerializeField] private Collider sensorCollider;

        public DropItem CurrentTarget => currentTarget;

        private void Awake()
        {
            sensorCollider = GetComponent<Collider>(); 
            sensorCollider.isTrigger = true;    // 센서는 반드시 trigger여야 함
        }

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<DropItem>();

            if (item != null && !detectedItems.Contains(item))
            {
                detectedItems.Add(item); 
                Debug.Log($"Item detected: {item}");
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var item = other.GetComponent<DropItem>();
            if (item != null && detectedItems.Contains(item))
            {
                detectedItems.Remove(item);
                if (currentTarget == item)
                {
                    currentTarget = null;
                }
            }
        }

        private void Update()
        {
            UpdateNearestItem();
        }

        private void UpdateNearestItem()
        {
            float minDistance = float.MaxValue;
            DropItem nearest = null;

            foreach (var item in detectedItems)
            {
                if (item == null) continue;

                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = item;
                }
            }

            // 가장 가까운 대상 갱신
            if (currentTarget != nearest)
            {
                Debug.Log($"Nearest item changed: {nearest}");
                currentTarget = nearest;
                // TODO: 여기서 UIManager나 PlayerController에
                // "대상 변경됨" 이벤트 알릴 수도 있음
            }
        }

    }
}
