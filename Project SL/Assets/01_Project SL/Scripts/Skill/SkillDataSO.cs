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

    [CreateAssetMenu(fileName = "ProjectileData", menuName = "PROJECT KYM/SkillData_Sub/ProjectileData")]
    public class ProjectileDataSO : ScriptableObject 
    {
        public string projectileID;

        public Projectile projectilePrefab;
        public float speed;
        public int baseProjectileCount; // 기본 발사체 개수
        public int extraProjectilePerLevel; // 레벨당 추가 발사체 개수
        public float baseDamage; // 기본 데미지
        public float extraDamagePerLevel; // 레벨당 추가 데미지
        public float lifeTime; // 발사체 지속 시간
    }

    [CreateAssetMenu(fileName = "HealData", menuName = "PROJECT KYM/SkillData_Sub/HealData")]
    public class HealDataSO : ScriptableObject 
    {
        public string healID;

        public float healAmount;
    }


    [CreateAssetMenu(fileName = "SkillData", menuName = "PROJECT KYM/SkillData")]
    public class SkillDataSO : ScriptableObject
    {
        [Header("Common")]
        public string SkillID;
        public string SkillName;
        public Sprite SkillIcon;

        public SkillType SkillType;
        public float Cooldown;
        public float SkillCost; // 아마 SP cost
        public string AnimationTriggerName; // 스킬 애니메이션 트리거 이름, 나중에 혹시 필요해질지도?

        [Header("SubSkillDataSO")]
        public ScriptableObject SkillData;
    }
}
