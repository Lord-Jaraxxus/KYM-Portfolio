using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public interface IHittable
    {
        void OnHit(float damage); // 피격 시 호출되는 메서드
    }
}
