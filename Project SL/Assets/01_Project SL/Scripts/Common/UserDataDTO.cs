using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        [System.Serializable]
        public class PlayerItemData 
        {
            [field: SerializeField] public string ItemID { get; private set; }
            [field: SerializeField] public int ItemCount { get; private set; }

            public PlayerItemData(string itemID, int itemCount) 
            {
                ItemID = itemID;
                ItemCount = itemCount;
            }
            public void IncreaseItemCount(int count) 
            {
                ItemCount += count;
            }
            public void DecreaseItemCount(int count) 
            {
                ItemCount -= count;
            }
        }

        public List<PlayerItemData> PlayerItems { get; private set; } = new();

        public void AddItem(string itemID, int itemCount) 
        {
            var existingItem = PlayerItems.Find(item => item.ItemID == itemID);
            if (existingItem != null)
            {
                existingItem.IncreaseItemCount(itemCount);
            }
            else
            {
                PlayerItems.Add(new PlayerItemData(itemID, itemCount));
            }
        }
        public void RemoveItem(string itemID, int count) 
        {
            var existingItem = PlayerItems.Find(item => item.ItemID == itemID);

            if (existingItem != null)
            {
                existingItem.DecreaseItemCount(count);
            }
            else return;

            if (existingItem.ItemCount <= 0)
            {
                PlayerItems.Remove(existingItem);
            }
        }
    }
}
