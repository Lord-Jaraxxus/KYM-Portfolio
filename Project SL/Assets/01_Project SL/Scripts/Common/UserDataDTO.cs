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

        [field: SerializeField] public List<PlayerItemData> PlayerItems { get; private set; } = new();

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
        public PlayerItemData RemoveItem(string itemID, int count)
        {
            PlayerItemData existingItem = PlayerItems.Find(item => item.ItemID == itemID);

            if (existingItem != null)   // null 검사 후 갯수 깎기
            {
                existingItem.DecreaseItemCount(count);
            }
            else
            {
                Debug.Log("existingItem is null");
                return null;
            }

            if (existingItem.ItemCount <= 0) // 수량이 0개 이하면 PlayerItems 리스트에서 아이템을 삭제
            {
                PlayerItems.Remove(existingItem);
            }

            return existingItem;    // 수량이 0개던 남았던 일단 아이템 데이터는 넘겨줘야 하므로
        }
    }

    [System.Serializable]
    public class PlayerShopDTO : UserDataDTO // 상점들의 아이템 재고 데이터를 담는 DTO
    {
        [System.Serializable]
        public class ItemStock // 아이템 한 종류의 재고 데이터
        {
            public string ItemID { get; private set; }
            public int ItemCount { get; private set; }

            public ItemStock(string itemID, int itemCount)
            {
                ItemID = itemID;
                ItemCount = itemCount;
            }
            public void IncreaseStock(int amount)
            {
                ItemCount += amount;
            }
            public bool DecreaseStock(int amount)
            {
                if (ItemCount < amount)
                    return false;

                ItemCount -= amount;
                return true;
            }
        }

        [System.Serializable]
        public class ShopData // 상점 하나의 재고 데이터
        {
            public string ShopID { get; private set; } // 상점 ID
            public List<ItemStock> ItemStocks { get; private set; } = new List<ItemStock>(); // 아이템 재고 리스트

            public ShopData(string shopID)
            {
                ShopID = shopID;
            }
        }

        [field: SerializeField] public List<ShopData> ShopDatas { get; private set; } = new List<ShopData>(); // 상점들의 재고 데이터 리스트
    }

    [System.Serializable]
    public class PlayerEconomyDTO : UserDataDTO
    {
        [field: SerializeField] public int Gold { get; private set; } = 0; // 시작할때는 0골드

        public void AddGold(int amount)
        {
            Gold += amount;
        }
        public void SubtractGold(int amount)
        {
            if (Gold < amount) { return; } // Gold가 부족할때 이벤트가 또 있어야하는데..

            Gold -= amount;
        }
    }

    [System.Serializable]
    public class PlayerEquipDTO : UserDataDTO
    {
        public class PlayerEquipSlotData
        {
            public EquipSlotType SlotType;
            public string EquippedItemID; // 장착된 아이템 ID
            public ItemDataSO EquipedItemDataSO; // 장착된 아이템 데이터 SO
        }
        public List<PlayerEquipSlotData> PlayerEquipSlots = new List<PlayerEquipSlotData>();  // 이거 그냥 5개만 따로 변수로 만들어놓으면 안댐? 괜히 나중에 헷갈릴수도 잇는데

        public void EquipItem(ItemDataSO itemDataSO) // 이게 맞나.... 일단 임시로라도. 나중에 장비 갈아끼울때 고쳐야할듯
        {
            PlayerEquipSlotData playerEquipSlotData = new PlayerEquipSlotData();
            playerEquipSlotData.EquipedItemDataSO = itemDataSO;
            playerEquipSlotData.SlotType = itemDataSO.EquipSlotType;
            playerEquipSlotData.EquippedItemID = itemDataSO.ItemID;
        }
    }
}
