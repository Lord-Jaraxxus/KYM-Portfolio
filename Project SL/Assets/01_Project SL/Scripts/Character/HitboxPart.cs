using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    enum HitboxPartType // 일단 머리 몸통 팔다리만 구분, 나중에 부파같은거 추가하면 더 세세하게 갈지도
    {
        Head,
        Body,
        Limb
    }

    public class HitboxPart : MonoBehaviour, IHittable
    {
        public CharacterBase Owner => owner;
        private CharacterBase owner;
        [SerializeField] private HitboxPartType hitboxPartType;

        public void OnHit(float damage)
        {
            float finalDamage = damage;

            switch (hitboxPartType)
            {
                case HitboxPartType.Head:
                    finalDamage *= 1.5f; // 머리 공격시 1.5배 데미지
                    break;
                case HitboxPartType.Body:
                    finalDamage *= 1.0f; // 몸통은 기본 데미지
                    break;
                case HitboxPartType.Limb:
                    finalDamage *= 0.8f; // 팔다리는 0.8배 데미지
                    break;
            }

            owner.OnHit(finalDamage);
        }

        public float TakeDamage(float damage) // 일단 여기선 쓰이는 곳 없음
        {
            return damage;
        }

        void Start()
        {
            owner = GetComponentInParent<CharacterBase>();
        }
        
    }
}
