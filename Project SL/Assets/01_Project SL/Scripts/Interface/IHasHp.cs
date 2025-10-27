using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface IHasHp
    {
        float CurHP { get; } // ���� ü��
        float MaxHP { get; } // �ִ� ü��

        float TakeDamage(float damage); // �������� �޴� �޼���
        void Heal(float amount); // ü���� ȸ���ϴ� �޼���
    }
}
