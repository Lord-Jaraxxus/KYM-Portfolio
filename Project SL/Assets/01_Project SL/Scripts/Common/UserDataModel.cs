using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KYM
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        public const string EditorUserDataPath = "Assets/01_Project KYM/Anothers/UserData/";
        [field: SerializeField] public PlayerInfoDto PlayerInfoDto { get; private set; } = new();
        [field: SerializeField] public PlayerItemDTO PlayerItemDto { get; private set; } = new();
        [field: SerializeField] public PlayerEconomyDTO PlayerEconomyDTO { get; private set; } = new();


        public event Action<PlayerItemDTO.PlayerItemData> OnInventoryUpdated;
        public event Action<int> OnEconomyUpdated; // 골드 등 재화 정보 변경시 이벤트

        public void Initialize()
        {
            // 아직 플레이어 위치라던가, 아무튼 세이브 데이터가 없으니 주석처리

            // LoadData<PlayerInfoDto>(out PlayerInfoDto loadPlayerInfoDto);
            // PlayerInfoDto = loadPlayerInfoDto;

            AddGold(5000); // 일단 이렇게 초기화, 나중에 세이브데이터로 바꾸기
        }

        public void LoadData<T>(out T loadData) where T : UserDataDTO, new()
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

        public void SaveData<T>(T data) where T : UserDataDTO
        {
            string jsonFormat = JsonUtility.ToJson(data, true);
            string savePath = $"{EditorUserDataPath}/{typeof(T)}.json";

            FileManager.WriteFileFromString(savePath, jsonFormat);
        }

        public void AddItem(string itemID, int itemCount)
        {
            PlayerItemDTO.PlayerItemData changedData = PlayerItemDto.AddItem(itemID, itemCount);
            OnInventoryUpdated?.Invoke(changedData);
        }

        public void AddGold(int amount)
        {
            PlayerEconomyDTO.AddGold(amount);

            int playerGoldAmount = PlayerEconomyDTO.Gold;
            OnEconomyUpdated?.Invoke(playerGoldAmount);

            Debug.Log($"[UserDataModel] Added {amount} Gold. New Balance: {playerGoldAmount}");
        }
        public void SubtractGold(int amount)
        {
            PlayerEconomyDTO.SubtractGold(amount);

            int playerGoldAmount = PlayerEconomyDTO.Gold;
            OnEconomyUpdated?.Invoke(playerGoldAmount);
        }
    }
}
