using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface IHittable
    {
        void OnHit(float damage); // �ǰ� �� ȣ��Ǵ� �޼���
    }
}
