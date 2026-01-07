using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class DeathEventHandler : SingletonBase<DeathEventHandler>
    {
        public void OnReceiveDeathEvent(bool isPlayerCharacter, string characterID) 
        {
            if(isPlayerCharacter) // 플레이어 캐릭터일 경우
            {
                Debug.Log("Player character has died. Character ID: " + characterID);
            }
            else // 적 캐릭터일 경우
            {
                EnemyDataSO enemyDataSO = GameDataModel.Singleton.EnemyDataDto.EnemyDatas.Find(x => x.StatData.ID == characterID);

                // TODO : 아이템, 골드, 경험치 등등 처치 보상 처리

                int goldReward = Random.Range(enemyDataSO.GoldRewardRange.x, enemyDataSO.GoldRewardRange.y + 1); // 골드 보상 (최소~최대 사이 랜덤 지급)
                UserDataModel.Singleton.AddGold(goldReward); // 골드 보상 지급

                // UserDataModel.Singleton.AddExp(enemyDataSO.ExpReward); // 경험치 보상 지급

                // 아이템 드랍 처리
                foreach (DropItemData dropData in enemyDataSO.DropTable)
                {
                    float roll = Random.value; // 0 ~ 1

                    if (roll <= dropData.DropChance)
                    {
                        int count = Random.Range(dropData.MinCount, dropData.MaxCount + 1);

                        for (int i = 0; i < count; i++)
                        {
                            ItemSystem.DropItem(dropData.Item.ItemID);
                        }
                    }
                }

                Debug.Log("NPC has died. Character ID: " + characterID);
            }
        }
    }
}
