using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
        [field: SerializeField] public PlayerSkillDto PlayerSkillDto { get; private set; } = new();

        public event Action<PlayerItemDto.PlayerItemData> OnInventoryUpdated;
        public event Action<int /*CurExp*/, int /*ReqExp*/> OnExpUpdated; // 경험치 정보 변경시 이벤트
        public event Action<int /*level*/, int /*levelUpCount*/> OnLevelUpdated; // 레벨 변경(레벨업)시 이벤트
        public event Action<int> OnEconomyUpdated; // 골드 등 재화 정보 변경시 이벤트
        public event Action<string /*skillID*/, int /*newSkillLevel*/> OnSkillLevelUp; // 스킬 레벨업시 이벤트

        public void Initialize()
        {
            // 아직 플레이어 위치라던가, 아무튼 세이브 데이터가 없으니 주석처리
            // LoadData<PlayerInfoDto>(out PlayerInfoDto loadPlayerInfoDto);
            // PlayerInfoDto = loadPlayerInfoDto;

            PlayerInfoDto.SetLevelAndExp(1, 0, 100); // 일단 레벨 1, 경험치 0, 요구 exp는 100으로 초기화

            AddGold(5000); // 일단 이렇게 골드 초기화, 나중에 세이브데이터로 바꾸기

            // 상점 데이터 초기화, 일단 GameDataModel에 있는 상점 데이터로 초기화 (SO 그대로)
            foreach (ShopDataSO shopData in GameDataModel.Singleton.ShopDataDto.ShopDatas) 
            {
                PlayerShopDto.ShopData newShopData = new PlayerShopDto.ShopData(shopData.ShopID); // 생성자로 초기화 

                foreach (ShopItemEntry itemEntry in shopData.ItemEntries)
                {
                    PlayerShopDto.ItemStock newItemStock = new PlayerShopDto.ItemStock(itemEntry.itemDataSO.ItemID, itemEntry.initialStock); // 생성자로 초기화
                    newShopData.ItemStocks.Add(newItemStock);
                }

                PlayerShopDto.ShopDatas.Add(newShopData);
            }

            // 플레이어 스킬 초기화, 게임데이터에서 로딩한 모든 스킬들을 일단 0레벨로 들고있도록 함
            foreach (var keyValuePair in GameDataModel.Singleton.SkillDatabase.SkillDatas) 
            {
                PlayerSkillDto.PlayerSkillData playerSkillData = new PlayerSkillDto.PlayerSkillData();
                playerSkillData.SkillID = keyValuePair.Key;
                playerSkillData.SkillLevel = 0; // 처음에는 모든 스킬 레벨 0
                PlayerSkillDto.PlayerSkills.Add(playerSkillData);
            }

            // 테스트용으로 일단 스킬 하나는 레벨 1로 시작하고 Q스킬에 장착
            PlayerSkillDto.PlayerSkillData playerSkillData1 = PlayerSkillDto.PlayerSkills.Find(playerSkillData1 => playerSkillData1.SkillID == "1");
            playerSkillData1.SkillLevel = 1; // 일단 1번 스킬(매직 미사일)은 레벨 1로 시작
            PlayerSkillDto.SetQSkillID(playerSkillData1.SkillID); // 일단 Q스킬에 장착
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

        public int CalculateRequiredExp(int level)
        {
            int requiredExp = level * 100; // 요구 경험치 계산, 예시로 레벨 * 100으로 설정
            return requiredExp;
        }

        public void AddExp(int exp)
        {
            int currentExp = PlayerInfoDto.CurrentExp + exp;
            int requiredExp = PlayerInfoDto.RequiredExp;
            int level = PlayerInfoDto.Level;
            int levelUpCount = 0;

            while (currentExp >= requiredExp)
            {
                currentExp -= requiredExp;
                level++;
                levelUpCount++;
                requiredExp = CalculateRequiredExp(level); // 요구 경험치 변경
            }
            // 레벨업 및 경험치 변경 이벤트 호출
            PlayerSkillDto.AddSkillPoint(levelUpCount); // 레벨업 횟수만큼 스킬 포인트 추가, 서순때문에 OnLevelUpdated 호출 전에 실행해야함
            OnLevelUpdated?.Invoke(level, levelUpCount);
            OnExpUpdated?.Invoke(currentExp, requiredExp);

            PlayerInfoDto.SetLevelAndExp(level, currentExp, requiredExp); // UserDataDto에 변경된 값 저장, UI에서 이전의 값들을 가져다 쓸 수 있도록 갱신은 이벤트 다 보낸 후 마지막에

            Debug.Log($"[UserDataModel] Added {exp} EXP. New Level: {level}, Current EXP: {currentExp}/{requiredExp}");
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
        public void UpdateEquipedItemData(ItemDataSO itemDataSO)
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

        public int SkillLevelUp(string skillID)
        {
            PlayerSkillDto.PlayerSkillData playerSkillData = PlayerSkillDto.PlayerSkills.FirstOrDefault(i => i.SkillID == skillID);
            
            if(playerSkillData != null) 
            {
                if (PlayerSkillDto.SkillPoints > 0) 
                {
                    playerSkillData.SkillLevel += 1; // 스킬 레벨 1 증가
                    UserDataModel.Singleton.PlayerSkillDto.TrySpendSkillPoint(1); // 스킬 포인트 1 차감
                    Debug.Log($"[UserDataModel] Skill ID {skillID} leveled up to {playerSkillData.SkillLevel}. Remaining Skill Points: {PlayerSkillDto.SkillPoints}");
                    OnSkillLevelUp?.Invoke(skillID, playerSkillData.SkillLevel); // 스킬 레벨업 이벤트 호출
                    return playerSkillData.SkillLevel;
                }
                else 
                {
                    Debug.LogWarning($"[UserDataModel] Not enough skill points to level up Skill ID {skillID}.");
                    return playerSkillData.SkillLevel; // 스킬 포인트가 부족하면 레벨 변화 없음
                }
            }
            else 
            {
                Debug.LogWarning($"[UserDataModel] Skill ID {skillID} not found in PlayerSkillDto.");
                return 0; // 스킬을 가지고 있지 않다면 레벨 0 반환
            }
        }

        public int GetSkillLevel(string skillID) 
        {
            PlayerSkillDto.PlayerSkillData playerSkillData = PlayerSkillDto.PlayerSkills.FirstOrDefault(i => i.SkillID == skillID);
            
            if(playerSkillData != null) 
            {
                return playerSkillData.SkillLevel;
            }
            else 
            {
                Debug.LogWarning($"[UserDataModel] Skill ID {skillID} not found in PlayerSkillDto.");
                return 0; // 스킬을 가지고 있지 않다면 레벨 0 반환
            }
        }
    }
}
