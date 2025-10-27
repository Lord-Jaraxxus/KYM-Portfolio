using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Weapon : MonoBehaviour
    {
        public float damage = 10f; // �������� �� �����̳� ���� ��� ��ų ��� ���������� ���ؾ� �ϱ� �ϴµ�
        private HashSet<IHittable> hitTargets = new HashSet<IHittable>(); // �̹� �ǰ��� ������ �����ϱ� ���� ����

        Collider hitbox;

        void Awake()
        {
            hitbox = GetComponent<Collider>();
            hitbox.enabled = false; // ó������ ��Ȱ��ȭ
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
            hitTargets.Clear(); // �� ���� ���� �� �ʱ�ȭ
            hitbox.enabled = true;
        }

        public void DisableHitbox()
        {
            hitbox.enabled = false;
        }

    }
}
