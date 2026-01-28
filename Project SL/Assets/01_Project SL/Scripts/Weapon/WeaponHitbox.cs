using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class WeaponHitbox : MonoBehaviour
    {
        [SerializeField] private CharacterBase weaponOwner; // 이 무기의 주인 캐릭터
        public CharacterBase HittedCharacter => hittedCharacter;

        public float damage = 10f; // 데미지는 뭐 스텟이나 강공 약공 스킬 등등 가변적으로 변해야 하긴 하는데
        private CharacterBase hittedCharacter = null; // 이 무기에 맞은 캐릭터
        Collider hitbox;

        void Awake()
        {
            weaponOwner = GetComponentInParent<CharacterBase>(); // 부모 캐릭터를 무기의 주인으로 설정

            hitbox = GetComponent<Collider>();
            if(hitbox != null) hitbox.enabled = false; // 처음에는 비활성화
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root == weaponOwner.transform.root) return; // 자기 자신 피격 방지

            // if (other.gameObject.layer == weaponOwner.gameObject.layer) return; // 아군 피격 방지 <- 이건 레이어를 아끼기 위해서 빼는게 좋다 하심

            HitboxPart part = other.GetComponent<HitboxPart>();
            if (part == null) return;

            // 아군 피격 방지: '맞은 캐릭터'의 태그 vs '무기 소유자' 태그
            if (part.Owner != null && part.Owner.CompareTag(weaponOwner.tag)) return;


            if (hittedCharacter == null || part.Owner != hittedCharacter ) // 일단 검증은 한번만, 나중엔 해쉬로 바꿔야할지도 (3개체 이상 동시에 때리면?)
            {
                hittedCharacter = part.Owner;
                part.OnHit(damage);

                Debug.Log("Hit: " + other.name);
            }
        }

        public void EnableHitbox()
        {
            if (hitbox != null) hitbox.enabled = true;
            else Debug.LogWarning("Hitbox Collider is missing!");
        }

        public void DisableHitbox()
        {
            hitbox.enabled = false;
            hittedCharacter = null; // 초기화
        }

    }
}
