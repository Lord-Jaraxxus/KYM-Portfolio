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

    [System.Serializable]
    public class ShopDataDTO : GameDataDTO
    {
        [field: SerializeField] public List<ShopDataSO> ShopDatas { get; private set; } = new(); // 상점 데이터 리스트
    }

    [System.Serializable]
    public class ItemDatabase : GameDataDTO // 아이템 정보 검색을 위해 ItemDataSO들을 로딩해서 들고있는 데이터베이스
    {
        [field: SerializeField] public Dictionary<string, ItemDataSO> ItemDatas { get; private set; } = new();

        public ItemDataSO GetItemDataSO(string itemID) 
        {
            // 받아온 PlayerItemData에서 ID를 가져와 Itembase에서 ID로 검색해서 해당 아이템의 itemDataSO를 가져옴, itemDataSO변수에 담김, 해당 ID의 아이템SO가 없으면 null 리턴
            if (!GameDataModel.Singleton.ItemDatabase.ItemDatas.TryGetValue(itemID, out ItemDataSO itemDataSO))
                return null;
            else 
                return itemDataSO;
        }
    }
}
