using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Weapon : MonoBehaviour
    {
        public float damage = 10f; // 데미지는 뭐 스텟이나 강공 약공 스킬 등등 가변적으로 변해야 하긴 하는데
        private HashSet<IHittable> hitTargets = new HashSet<IHittable>(); // 이미 피격한 대상들을 추적하기 위한 집합

        Collider hitbox;

        void Awake()
        {
            hitbox = GetComponent<Collider>();
            hitbox.enabled = false; // 처음에는 비활성화
        }

        private void OnTriggerEnter(Collider other)
        {
            IHittable hittable = other.GetComponent<IHittable>();
            if (hittable != null && !hitTargets.Contains(hittable)) 
            {
                hitTargets.Add(hittable);
                hittable.OnHit(damage);
            }
        }

        public void EnableHitbox()
        {
            hitTargets.Clear(); // 새 공격 시작 시 초기화
            hitbox.enabled = true;
        }

        public void DisableHitbox()
        {
            hitbox.enabled = false;
        }

    }
}
