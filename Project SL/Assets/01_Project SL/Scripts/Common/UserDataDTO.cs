using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class UserDataDto { }


    [System.Serializable]
    public class PlayerInfoDto : UserDataDto
    {
        [field: SerializeField] public Vector3 LastPosition { get; private set; }
        [field: SerializeField] public Vector3 LastRotation { get; private set; }
        [field: SerializeField] public float LastCurHP { get; private set; }
        [field: SerializeField] public float LastCurSP { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int CurrentExp { get; private set; }
        [field: SerializeField] public int RequiredExp { get; private set; }

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
        public void SetLevelAndExp(int level, int curExp, int reqExp)
        {
            this.Level = level;
            this.CurrentExp = curExp;
            this.RequiredExp = reqExp;
        }

        public void SaveData() => UserDataModel.Singleton.SaveData<PlayerInfoDto>(this);
    }

    [System.Serializable]
    public class PlayerItemDto : UserDataDto
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
    public class PlayerShopDto : UserDataDto // 상점들의 아이템 재고 데이터를 담는 DTO
    {
        [System.Serializable]
        public class ItemStock // 아이템 한 종류의 재고 데이터
        {
            [field: SerializeField] public string ItemID { get; private set; }
            [field: SerializeField] public int ItemCount { get; private set; }

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
            [field: SerializeField] public string ShopID { get; private set; } // 상점 ID
            // public List<(string /* Item ID */, int /* Item Count */)> ItemStocks;  // 아이템 재고 초기 데이터용 튜플 리스트, 이런 구조로 써도 된다
            [field: SerializeField] public List<ItemStock> ItemStocks { get; private set; } = new List<ItemStock>(); // 아이템 재고 리스트

            public ShopData(string shopID)
            {
                ShopID = shopID;
            }
        }

        [field: SerializeField] public List<ShopData> ShopDatas { get; private set; } = new List<ShopData>(); // 상점들의 재고 데이터 리스트
    }

    [System.Serializable]
    public class PlayerEconomyDto : UserDataDto
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
    public class PlayerEquipDto : UserDataDto
    {
        [System.Serializable]

        public class PlayerEquipSlotData
        {
            public EquipSlotType SlotType;
            public string EquippedItemID; // 장착된 아이템 ID
            public ItemDataSO EquipedItemDataSO; // 장착된 아이템 데이터 SO
        }
        [SerializeField] public List<PlayerEquipSlotData> PlayerEquipSlots = new List<PlayerEquipSlotData>();
    }

    [System.Serializable]
    public class PlayerSkillDto : UserDataDto
    {
        [System.Serializable]
        public class PlayerSkillData
        {
            public string SkillID;
            public int SkillLevel;
        }
        [SerializeField] public List<PlayerSkillData> PlayerSkills = new List<PlayerSkillData>(); // 현재 플레이어가 가진 스킬들에 대한 정보 리스트
        [field: SerializeField] public string QSkillID { get; private set; }
        [field: SerializeField] public string ESkillID { get; private set; }
        [field: SerializeField] public int SkillPoints { get; private set; }

        public void SetQSkillID(string skillID)
        {
            QSkillID = skillID;
        }
        public void SetESkillID(string skillID)
        {
            ESkillID = skillID;
        }
        public void AddSkillPoint(int amount)
        {
            SkillPoints += amount;
        }
        public bool TrySpendSkillPoint(int amount = 1)
        {
            if (SkillPoints < amount) return false;
            SkillPoints -= amount;
            return true;
        }
    }
}
