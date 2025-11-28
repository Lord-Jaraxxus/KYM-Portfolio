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
        [field: SerializeField] public Dictionary<string /* Item Name */, PlayerItemDTO> PlayerItemDtoDictionary { get; private set; } = new();

        public event Action<PlayerItemDTO.PlayerItemData> OnInventoryUpdated;

        public void Initialize()
        {
            LoadData<PlayerInfoDto>(out PlayerInfoDto loadPlayerInfoDto);
            PlayerInfoDto = loadPlayerInfoDto;
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

            Debug.Log("Current Player Items:");
            foreach (var item in PlayerItemDto.PlayerItems)
            {
                Debug.Log($"ItemID: {item.ItemID}, Count: {item.ItemCount}");
            }
        }
    }
}
