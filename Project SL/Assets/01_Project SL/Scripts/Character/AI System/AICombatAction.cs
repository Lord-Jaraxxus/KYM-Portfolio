using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public enum CombatActionType
    {
        MeleeAttack,     // 근접 공격
        ProjectileSkill, // 투사체 스킬
        KeepDistance,    // 너무 가까우면 뒤로 빠짐
    }

    [System.Serializable]
    public class AICombatAction
    {
        public string actionName = "New Action";
        public CombatActionType type = CombatActionType.MeleeAttack;

        [Header("Range")]
        public float minRange = 0f;
        public float maxRange = 2f;

        [Header("Timing")]
        public float cooldown = 1.0f;

        [Header("Selection")]
        public int priority = 0;       // 높을수록 우선
        public float weight = 1f;      // WeightedRandom 모드에서 사용

        [Header("Melee")]
        // [Tooltip("0=Attack1, 1=Attack2, 2=Attack3")]
        public int meleeAttackIndex = 0;

        [Header("Projectile")]
        public string skillId;         // Skill ID

        [Header("KeepDistance")]
        public float desiredDistance = 5f; // 이 거리보다 가까우면 뒤로 빠짐
        public float retreatStep = 2f;     // 한 번에 뒤로 빠지는 거리

        public bool IsInRange(float distance)
        {
            return distance >= minRange && distance <= maxRange;
        }
    }
}
