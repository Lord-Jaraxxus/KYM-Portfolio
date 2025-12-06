using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class GameDataDTO { }

    [System.Serializable]
    public class PlayerStatDto : GameDataDTO
    {
        [field: SerializeField] public CharacterStatDataSO playerCharacterStatSO { get; private set; } // 플레이어 캐릭터 스탯 데이터 (ScriptableObject)

        public void initailize(CharacterStatDataSO dataSO)
        {
            this.playerCharacterStatSO = dataSO;
        }
    }

    [System.Serializable]
    public class EnemyStatDto : GameDataDTO
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

}
