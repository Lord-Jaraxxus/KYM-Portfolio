using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field: SerializeField] public PlayerStatDto PlayerStatDto { get; private set; } = new(); // 플레이어 스탯 DTO
        [field: SerializeField] public EnemyStatDto EnemyStatDto { get; private set; } = new(); // 적 스탯 DTO
        [field: SerializeField] public EnemyDataDto EnemyDataDto { get; private set; } = new(); // 적 데이터 SO
        [field: SerializeField] public ShopDataDto ShopDataDto { get; private set; } = new(); // 상점 데이터 DTO
        [field: SerializeField] public ItemDatabase ItemDatabase { get; private set; } = new(); // 아이템 데이터베이스 

        public void Initialize()
        {
            CharacterStatDataSO playerStatSo = Resources.Load<CharacterStatDataSO>("Character/CharacterStat/PlayerCharacterStatData");

            // 적 캐릭터 스탯 데이터 로드
            CharacterStatDataSO[] arrEneyStatSO = Resources.LoadAll<CharacterStatDataSO>("Character/EnemyStat/");
            foreach (CharacterStatDataSO enemyStatSO in arrEneyStatSO) 
            {
                EnemyStatDto.EnemyStatData enemyData = new EnemyStatDto.EnemyStatData(enemyStatSO.ID, enemyStatSO);
                EnemyStatDto.EnemyDatas.Add(enemyData.EnemyID, enemyData);
                Debug.Log($"[GameDataModel] Enemy Stat Data Loaded: ID = {enemyData.EnemyID}");
            }

            // 적 캐릭터 데이터 로드
            EnemyDataSO[] arrEnemyDataSO = Resources.LoadAll<EnemyDataSO>("Character/EnemyData/");
            foreach (EnemyDataSO enemyDataSO in arrEnemyDataSO) 
            {
                EnemyDataDto.EnemyDatas.Add(enemyDataSO);
                Debug.Log($"[GameDataModel] Enemy Data SO Loaded: ID = {enemyDataSO.StatData.ID}");
            }

            // 상점 데이터 로드
            ShopDataSO[] arrShopDataSO = Resources.LoadAll<ShopDataSO>("Shop/");
            foreach (ShopDataSO shopDataSO in arrShopDataSO) 
            {
                ShopDataDto.ShopDatas.Add(shopDataSO);
            }

            // 아이템 데이터 로드
            ItemDataSO[] arrItemDataSO = Resources.LoadAll<ItemDataSO>("ItemData/");
            foreach (ItemDataSO itemDataSO in arrItemDataSO) 
            {
                ItemDatabase.ItemDatas.Add(itemDataSO.ItemID, itemDataSO); // ID로 검색 가능하도록 딕셔너리에 저장
            }

            PlayerStatDto.initailize(playerStatSo); // 플레이어 스탯 데이터 초기화
        }

        public ItemDataSO GetItemDataSO(string itemID)
        {
            // 받아온 PlayerItemData에서 ID를 가져와 Itembase에서 ID로 검색해서 해당 아이템의 itemDataSO를 가져옴, itemDataSO변수에 담김, 해당 ID의 아이템SO가 없으면 null 리턴
            if (!ItemDatabase.ItemDatas.TryGetValue(itemID, out ItemDataSO itemDataSO))
                return null;
            else
                return itemDataSO;
        }
    }
}
