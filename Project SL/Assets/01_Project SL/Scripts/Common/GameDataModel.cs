using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field: SerializeField] public PlayerStatDto PlayerStatDto { get; private set; } = new(); // 플레이어 스탯 DTO
        [field: SerializeField] public EnemyStatDto EnemyStatDto { get; private set; } = new(); // 적 스탯 DTO

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

            PlayerStatDto.initailize(playerStatSo); // 플레이어 스탯 데이터 초기화
        }
    }
}
