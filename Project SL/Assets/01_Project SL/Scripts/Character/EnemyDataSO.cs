using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "PROJECT KYM/EnemyData")]
    public class EnemyDataSO : ScriptableObject
    {
        public string EnemyID { get; private set; } // 적 캐릭터 ID
        public CharacterStatDataSO StatData { get; private set; } // ⭐핵심: 스탯은 참조
        public int ExpReward { get; private set; } // 경험치 보상
        public Vector2Int GoldRewardRange { get; private set; } // 골드 보상 범위 (최소, 최대)
        public List<DropItemData> DropTable { get; private set; } // 드롭 아이템 테이블
    }

    [System.Serializable]
    public class DropItemData
    {
        public ItemDataSO Item;
        public int MinCount;
        public int MaxCount;
        public float DropChance; // 0~1
    }
}

