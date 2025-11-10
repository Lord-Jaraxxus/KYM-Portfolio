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
}
