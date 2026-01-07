using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "PROJECT KYM/EnemyData")]
    public class EnemyDataSO : ScriptableObject
    {
        [field:SerializeField] public CharacterStatDataSO StatData { get; private set; } // ⭐핵심: 스탯은 참조
        [field: SerializeField] public int ExpReward { get; private set; } // 경험치 보상
        [field: SerializeField] public Vector2Int GoldRewardRange { get; private set; } // 골드 보상 범위 (최소, 최대)
        [field: SerializeField] public List<DropItemData> DropTable { get; private set; } // 드롭 아이템 테이블
    }

    [System.Serializable]
    public class DropItemData
    {
        public ItemDataSO ItemDataSO;
        public int MinCount;
        public int MaxCount;
        public float DropChance; // 0~1
    }
}

