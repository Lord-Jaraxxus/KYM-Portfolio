using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class DeathEventHandler : SingletonBase<DeathEventHandler>
    {
        public void OnReceiveDeathEvent(bool isPlayerCharacter, string characterID, Transform characterTransform)
        {
            if (isPlayerCharacter) // 플레이어 캐릭터일 경우
            {
                Debug.Log("Player character has died. Character ID: " + characterID);
            }
            else // 적 캐릭터일 경우
            {
                EnemyDataSO enemyDataSO = GameDataModel.Singleton.EnemyDataDto.EnemyDatas.Find(x => x.StatData.ID == characterID);

                // TODO : 아이템, 골드, 경험치 등등 처치 보상 처리

                int goldReward = Random.Range(enemyDataSO.GoldRewardRange.x, enemyDataSO.GoldRewardRange.y + 1); // 골드 보상 (최소~최대 사이 랜덤 지급)
                UserDataModel.Singleton.AddGold(goldReward); // 골드 보상 지급

                UserDataModel.Singleton.AddExp(enemyDataSO.ExpReward); // 경험치 보상 지급

                // 아이템 드랍 처리
                foreach (DropItemData dropData in enemyDataSO.DropTable)
                {
                    float roll = Random.value; // 0 ~ 1

                    if (dropData.DropChance <= roll)
                    {
                        int count = Random.Range(dropData.MinCount, dropData.MaxCount + 1); // 아이템 드랍 개수 결정

                        // TODO : 아이템 드랍 구현
                        // 1. 프리팹 소환
                        GameObject itemVisual = GameObject.Instantiate(
                            dropData.ItemDataSO.ItemVisualPrefab,
                            characterTransform.position,
                            Quaternion.identity);

                        // 2. DropItem 컴포넌트 초기화
                        DropItem dropItemComp = itemVisual.AddComponent<DropItem>();
                        dropItemComp.Initialize(dropData.ItemDataSO, count);
                    }
                }

                Debug.Log("NPC has died. Character ID: " + characterID);
            }
        }
    }
}
