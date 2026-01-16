using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public enum SkillType
    {
        Projectile,
        Melee,
        Buff,
        Heal, 
    }
    [Serializable]
    public struct ProjectileSkillData
    {
        public Projectile projectilePrefab;
        public float speed;
        public float damage;
        public float lifeTime;
    }
    [Serializable]
    public struct HealSkillData
    {
        public float healAmount;
    }

    [CreateAssetMenu(fileName = "SkillData", menuName = "PROJECT KYM/SkillData")]

    public class SkillDataSO : ScriptableObject
    {
        [Header("Common")]
        public string SkillID;
        public string SkillName;
        public SkillType SkillType;
        public float Cooldown;
        public float SkillCost; // 아마 SP cost
        public string AnimationTriggerName; // 스킬 애니메이션 트리거 이름, 나중에 혹시 필요해질지도?

        [Header("Projectile")]
        public ProjectileSkillData ProjectileData;

        [Header("Heal")]
        public HealSkillData HealData;
    }
}
