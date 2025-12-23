using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field: SerializeField] public PlayerStatDto PlayerStatDto { get; private set; } = new(); // 플레이어 스탯 DTO
        [field: SerializeField] public EnemyStatDto EnemyStatDto { get; private set; } = new(); // 적 스탯 DTO
        [field: SerializeField] public ShopDataDTO ShopDataDTO { get; private set; } = new(); // 상점 데이터 DTO
        [field: SerializeField] public ItemDatabase ItemDatabase { get; private set; } = new(); // 아이템 데이터베이스 

        public void Initialize()
        {
            CharacterStatDataSO playerStatSo = Resources.Load<CharacterStatDataSO>("Character/CharacterStat/PlayerCharacterStatData");

            CharacterStatDataSO[] arrEneyStatSO = Resources.LoadAll<CharacterStatDataSO>("Character/EnemyStat/");
            foreach (CharacterStatDataSO enemyStatSO in arrEneyStatSO) 
            {
                EnemyStatDto.EnemyStatData enemyData = new EnemyStatDto.EnemyStatData(enemyStatSO.ID, enemyStatSO);
                EnemyStatDto.EnemyDatas.Add(enemyData.EnemyID, enemyData);
                Debug.Log($"[GameDataModel] Enemy Stat Data Loaded: ID = {enemyData.EnemyID}");
            }

            ShopDataSO[] arrShopDataSO = Resources.LoadAll<ShopDataSO>("Shop/");
            foreach (ShopDataSO shopDataSO in arrShopDataSO) 
            {
                ShopDataDTO.ShopDatas.Add(shopDataSO);
            }

            ItemDataSO[] arrItemDataSO = Resources.LoadAll<ItemDataSO>("ItemData/");
            foreach (ItemDataSO itemDataSO in arrItemDataSO) 
            {
                ItemDatabase.ItemDatas.Add(itemDataSO.ItemID, itemDataSO); // ID로 검색 가능하도록 딕셔너리에 저장
            }

            PlayerStatDto.initailize(playerStatSo); // 플레이어 스탯 데이터 초기화
        }
    }
}
