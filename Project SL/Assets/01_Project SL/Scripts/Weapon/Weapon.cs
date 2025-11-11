using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Weapon : MonoBehaviour
    {
        public float damage = 10f; // 데미지는 뭐 스텟이나 강공 약공 스킬 등등 가변적으로 변해야 하긴 하는데
        public CharacterBase hittedCharacter = null; // 이 무기에 맞은 캐릭터
        Collider hitbox;

        void Awake()
        {
            hitbox = GetComponent<Collider>();
            if(hitbox != null) hitbox.enabled = false; // 처음에는 비활성화
        }

        private void OnTriggerEnter(Collider other)
        {
            HitboxPart part = other.GetComponent<HitboxPart>();
            if (part == null) return; 

            if (hittedCharacter == null || part.Owner != hittedCharacter ) // 일단 검증은 한번만, 나중엔 해쉬로 바꿔야할지도 (3개체 이상 동시에 때리면?)
            {
                hittedCharacter = part.Owner;
                part.OnHit(damage);

                Debug.Log("Hit: " + other.name);
            }
        }

        public void EnableHitbox()
        {
            hitbox.enabled = true;
        }

        public void DisableHitbox()
        {
            hitbox.enabled = false;
            hittedCharacter = null; // 초기화
        }

    }
}
