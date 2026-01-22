using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public static class DamageCalculator 
    {
        public static float CalculateDamage(float attackerPower, float defenderDefence)
        {
            // 방어력 계산, 일단 방어력만큼 고정값으로 데미지 깎이도록, 나중에 퍼센트 계산으로 갈수도?
            float finalDamage = Mathf.Max(0f, attackerPower - defenderDefence); // 0보다 작아지면 안되니까 방어력 뺀 값이 음수면 0으로 처리
            return finalDamage;
        }
    }
}
