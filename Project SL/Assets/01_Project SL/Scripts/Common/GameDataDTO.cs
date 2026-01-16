using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class GameDataDto { }

    [System.Serializable]
    public class PlayerStatDto : GameDataDto
    {
        [field: SerializeField] public CharacterStatDataSO playerCharacterStatSO { get; private set; } // 플레이어 캐릭터 스탯 데이터 (ScriptableObject)

        public void initailize(CharacterStatDataSO dataSO)
        {
            this.playerCharacterStatSO = dataSO;
        }
    }

    [System.Serializable]
    public class EnemyStatDto : GameDataDto
    {
        [System.Serializable]
        public class EnemyStatData
        {
            public EnemyStatData(string enemyID, CharacterStatDataSO enemyStat)
            {
                EnemyID = enemyID;
                EnemyStat = enemyStat;
            }

            [field: SerializeField] public string EnemyID { get; private set; } // 적 캐릭터 ID
            [field: SerializeField] public CharacterStatDataSO EnemyStat { get; private set; } // 적 캐릭터 스탯 데이터 (ScriptableObject)
        }

        [field: SerializeField] public UDictionary<string, EnemyStatData> EnemyDatas { get; private set; } = new();  // 적 캐릭터 스탯 데이터 리스트

        public EnemyStatData GetEnemyStatData(string enemyID)
        {
            if (EnemyDatas.TryGetValue(enemyID, out EnemyStatData enemyStatData))
            {
                return enemyStatData;
            }
            else
            {
                Debug.LogError($"[EnemyStatDto] Enemy Stat Data not found for ID: {enemyID}");
                return null;
            }
        }
    }

    [System.Serializable]
    public class EnemyDataDto : GameDataDto
    {
        [field: SerializeField] public List<EnemyDataSO> EnemyDatas { get; private set; } = new(); // 적 데이터 리스트
    }


    [System.Serializable]
    public class ShopDataDto : GameDataDto
    {
        [field: SerializeField] public List<ShopDataSO> ShopDatas { get; private set; } = new(); // 상점 데이터 리스트
    }

    [System.Serializable]
    public class ItemDatabase : GameDataDto // 아이템 정보 검색을 위해 ItemDataSO들을 로딩해서 들고있는 데이터베이스
    {
        [field: SerializeField] public Dictionary<string, ItemDataSO> ItemDatas { get; private set; } = new();
    }

    [System.Serializable]
    public class SkillDatabase : GameDataDto // 스킬 정보 검색을 위해 SkillDataSO들을 로딩해서 들고있는 데이터베이스
    {
        [field: SerializeField] public Dictionary<string, SkillDataSO> SkillDatas { get; private set; } = new();
    }
}
