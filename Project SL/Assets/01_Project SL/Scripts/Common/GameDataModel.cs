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

            PlayerStatDto.initailize(playerStatSo); // 플레이어 스탯 데이터 초기화
        }
    }
}
