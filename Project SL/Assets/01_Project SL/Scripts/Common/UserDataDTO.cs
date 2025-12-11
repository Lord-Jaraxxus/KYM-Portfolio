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

    [System.Serializable]
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

        [field:SerializeField] public List<PlayerItemData> PlayerItems { get; private set; } = new();

        public PlayerItemData AddItem(string itemID, int itemCount) 
        {
            PlayerItemData existingItem = PlayerItems.Find(item => item.ItemID == itemID);
            if (existingItem != null)
            {
                existingItem.IncreaseItemCount(itemCount);
                return existingItem;
            }
            else
            {   
                PlayerItemData newItem = new PlayerItemData(itemID, itemCount);
                PlayerItems.Add(newItem);
                return newItem;
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

    public class PlayerEquipDTO : UserDataDTO
    {
        public class PlayerEquipSlotData 
        {
            public EquipSlotType SlotType;
            public string EquippedItemID; // 장착된 아이템 ID
        }
        public List<PlayerEquipSlotData> PlayerEquipSlots = new List<PlayerEquipSlotData>();  // 이거 그냥 5개만 따로 변수로 만들어놓으면 안댐? 괜히 나중에 헷갈릴수도 잇는데
    }
}
