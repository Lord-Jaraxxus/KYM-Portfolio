using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class UserDataDTO { }


    [System.Serializable]
    public class PlayerInfoDto : UserDataDTO
    {
        [field: SerializeField] public Vector3 LastPosition { get; private set; }
        [field: SerializeField] public Vector3 LastRotation { get; private set; }
        [field: SerializeField] public float LastCurHP { get; private set; }
        [field: SerializeField] public float LastCurSP { get; private set; }

        public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
        {
            this.LastPosition = pos;
            this.LastRotation = rot.eulerAngles;
        }
        public void SetLastCurHPSP(float hp, float sp)
        {
            this.LastCurHP = hp;
            this.LastCurSP = sp;
        }

        public void SaveData() => UserDataModel.Singleton.SaveData<PlayerInfoDto>(this);
    }

    public class PlayerItemDTO : UserDataDTO
    {
        // [field: SerializeField] public ItemDataSO itemDataSO;
        // [field: SerializeField] public int Count;
    }
}
