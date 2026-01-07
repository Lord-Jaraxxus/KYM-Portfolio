using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static KYM.PlayerEquipDto;

namespace KYM
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        public const string EditorUserDataPath = "Assets/01_Project KYM/Anothers/UserData/";
        [field: SerializeField] public PlayerInfoDto PlayerInfoDto { get; private set; } = new();
        [field: SerializeField] public PlayerItemDto PlayerItemDto { get; private set; } = new();
        [field: SerializeField] public PlayerEconomyDto PlayerEconomyDto { get; private set; } = new();
        [field: SerializeField] public PlayerShopDto PlayerShopDto { get; private set; } = new();
        [field: SerializeField] public PlayerEquipDto PlayerEquipDto { get; private set; } = new();

        public event Action<PlayerItemDto.PlayerItemData> OnInventoryUpdated; 
        public event Action<int> OnEconomyUpdated; // 골드 등 재화 정보 변경시 이벤트

        public void Initialize()
        {
            // 아직 플레이어 위치라던가, 아무튼 세이브 데이터가 없으니 주석처리
            // LoadData<PlayerInfoDto>(out PlayerInfoDto loadPlayerInfoDto);
            // PlayerInfoDto = loadPlayerInfoDto;

            AddGold(5000); // 일단 이렇게 골드 초기화, 나중에 세이브데이터로 바꾸기

            // 상점 데이터 초기화, 일단 GameDataModel에 있는 상점 데이터로 초기화 (SO 그대로)
            foreach (ShopDataSO shopData in GameDataModel.Singleton.ShopDataDto.ShopDatas) 
            {
                PlayerShopDto.ShopData newShopData = new PlayerShopDto.ShopData(shopData.ShopID); // 생성자로 초기화 

                foreach (ItemDataSO itemData in shopData.ItemsForSale)
                {
                    PlayerShopDto.ItemStock newItemStock = new PlayerShopDto.ItemStock(itemData.ItemID, itemData.ItemCount); // 생성자로 초기화
                    newShopData.ItemStocks.Add(newItemStock);
                }

                PlayerShopDto.ShopDatas.Add(newShopData);
            }
        }

        public void LoadData<T>(out T loadData) where T : UserDataDto, new()
        {
            string loadpath = $"{EditorUserDataPath}/{typeof(T)}.json";
            if (FileManager.ReadFileData(loadpath, out string receiveData))
            {
                loadData = JsonUtility.FromJson<T>(receiveData);
            }
            else
            {
                loadData = new T();
            }
        }

        public void SaveData<T>(T data) where T : UserDataDto
        {
            string jsonFormat = JsonUtility.ToJson(data, true);
            string savePath = $"{EditorUserDataPath}/{typeof(T)}.json";

            FileManager.WriteFileFromString(savePath, jsonFormat);
        }

        public void AddItem(string itemID, int itemCount)
        {
            PlayerItemDto.PlayerItemData changedData = PlayerItemDto.AddItem(itemID, itemCount);
            OnInventoryUpdated?.Invoke(changedData);
        }

        public void RemoveItem(string itemID, int itemCount) 
        {
            PlayerItemDto.PlayerItemData changedData = PlayerItemDto.RemoveItem(itemID, itemCount);

            if (changedData != null)
            {
                OnInventoryUpdated?.Invoke(changedData);
            }
            else
            {
                Debug.Log("ChangedData is null.");
            }
        }

        public void AddGold(int amount)
        {
            PlayerEconomyDto.AddGold(amount);

            int playerGoldAmount = PlayerEconomyDto.Gold;
            OnEconomyUpdated?.Invoke(playerGoldAmount);

            Debug.Log($"[UserDataModel] Added {amount} Gold. New Balance: {playerGoldAmount}");
        }
        public void SubtractGold(int amount)
        {
            PlayerEconomyDto.SubtractGold(amount);

            int playerGoldAmount = PlayerEconomyDto.Gold;
            OnEconomyUpdated?.Invoke(playerGoldAmount);
        }

        public PlayerEquipSlotData GetSameSlotEquip(EquipSlotType equipSlotType) 
        {
            PlayerEquipSlotData sameEquipSlotData = PlayerEquipDto.PlayerEquipSlots.FirstOrDefault(i => i.SlotType == equipSlotType);

            return sameEquipSlotData;
        }
        public void UpdateEquipedItemData(ItemDataSO itemDataSO) // 이게 맞나.... 일단 임시로라도. 나중에 장비 갈아끼울때 고쳐야할듯
        {
            // 같은 슬롯의 장비를 이미 장착하고 있다면, 변수로 가져옴.
            PlayerEquipSlotData sameEquipSlotData = GetSameSlotEquip(itemDataSO.EquipSlotType);
            if (sameEquipSlotData == null) // 같은 슬롯의 장비가 없었다면, 즉 빈 슬롯에 장비를 끼우는 거라면 
            {
                // 새로 슬롯 데이터를 만들어서 리스트에 넣어줌
                PlayerEquipSlotData playerEquipSlotData = new PlayerEquipSlotData();
                playerEquipSlotData.EquipedItemDataSO = itemDataSO;
                playerEquipSlotData.SlotType = itemDataSO.EquipSlotType;
                playerEquipSlotData.EquippedItemID = itemDataSO.ItemID;

                PlayerEquipDto.PlayerEquipSlots.Add(playerEquipSlotData);
            }
            else // 이미 같은 타입의 장비를 끼고 있었다면
            {
                // 슬롯 데이터만 갱신
                sameEquipSlotData.EquipedItemDataSO = itemDataSO;
                sameEquipSlotData.SlotType = itemDataSO.EquipSlotType;
                sameEquipSlotData.EquippedItemID = itemDataSO.ItemID;
            }
        }
        public void UneqiupItem(ItemDataSO itemDataSO) 
        {
            // 해제할 장비 슬롯의 데이터를 가져옴
            PlayerEquipSlotData sameEquipSlotData = GetSameSlotEquip(itemDataSO.EquipSlotType);
            
            PlayerEquipDto.PlayerEquipSlots.Remove(sameEquipSlotData); // 해당 장비 슬롯의 데이터를 리스트에서 삭제
        }


    }
}
