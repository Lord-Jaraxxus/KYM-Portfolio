using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public class IngameScene : SceneBase
    {
        public override bool IsAdditiveScene => false; // Ingame 씬은 단일 씬으로 로드됨

        public override IEnumerator OnStart()
        {
            var asyncSceneLoad = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), this.LoadSceneMode);
            while (!asyncSceneLoad.isDone)
            {
                yield return null; // 씬 로드가 완료될 때까지 대기
            }

            UIManager.Show<PlayerHUD>(UIList.PlayerHUD); // Player HUD UI 표시
            UIManager.Show<GlobalUI>(UIList.GlobalUI); // Global UI 표시
            UIManager.Hide<InventoryUI>(UIList.InventoryUI); // Inventory UI 숨기고 시작

            //// UI - PlayerHUD 초기화
            //var playerHUD = UIManager.Singleton.GetUI<PlayerHUD>(UIList.PlayerHUD);
            //playerHUD.RefreshHpUI(PlayerController.Instance.LinkedCharacter.CurHP, PlayerController.Instance.LinkedCharacter.MaxHP);
            //playerHUD.RefreshSpUI(PlayerController.Instance.LinkedCharacter.CurSP, PlayerController.Instance.LinkedCharacter.MaxSP);
            //playerHUD.RefreshGoldUI(UserDataModel.Singleton.PlayerEconomyDto.Gold);
            //// UI - 상점 UI 초기화
            //var shopUI = UIManager.Singleton.GetUI<ShopUI>(UIList.ShopUI);
            //// UI - 캐릭터 장비창 UI 초기화
            //var characterEquipUI = UIManager.Singleton.GetUI<CharacterEquipUI>(UIList.CharacterEquipUI);
            //// UI - 캐릭터 정보창 UI 초기화
            //var characterInfoUI = UIManager.Singleton.GetUI<CharacterInfoUI>(UIList.CharacterInfoUI);
            //characterInfoUI.Initialize(); // 캐릭터 정보창 UI 초기화

            //// 이벤트 구독
            //PlayerController.Instance.LinkedCharacter.OnHpChanged += playerHUD.RefreshHpUI;   // 플레이어 캐릭터 HP 변경시 HUD 갱신
            //PlayerController.Instance.LinkedCharacter.OnSpChanged += playerHUD.RefreshSpUI;   // 플레이어 캐릭터 SP 변경시 HUD 갱신
            //UserDataModel.Singleton.OnEconomyUpdated += playerHUD.RefreshGoldUI; // 골드 변경시 HUD 갱신
            //shopUI.OnShopClosed += PlayerController.Instance.LinkedCharacter.SetCharacterState; // 상점 닫기 버튼 클릭시 (상점 닫힐시) 플레이어 상태 변경
        }

        public override IEnumerator OnEnd()
        {
            UIManager.Hide<PlayerHUD>(UIList.PlayerHUD); // Player HUD UI 숨김
            UIManager.Hide<GlobalUI>(UIList.GlobalUI); // Global UI 숨김

            var playerHUD = UIManager.Singleton.GetUI<PlayerHUD>(UIList.PlayerHUD);
            var shopUI = UIManager.Singleton.GetUI<ShopUI>(UIList.ShopUI);
            var characterEquipUI = UIManager.Singleton.GetUI<CharacterEquipUI>(UIList.CharacterEquipUI);

            //// UI 이벤트 구독 해제
            //PlayerController.Instance.LinkedCharacter.OnHpChanged -= playerHUD.RefreshHpUI;
            //PlayerController.Instance.LinkedCharacter.OnSpChanged -= playerHUD.RefreshSpUI;
            //UserDataModel.Singleton.OnEconomyUpdated -= playerHUD.RefreshGoldUI;
            //shopUI.OnShopClosed -= PlayerController.Instance.LinkedCharacter.SetCharacterState;
            //PlayerController.Instance.LinkedCharacter.OnEquipChanged -= characterEquipUI.SetIcon;

            // 씬 종료 시 필요한 작업이 있다면 여기에 추가
            yield return null; // 현재는 특별한 작업이 없으므로 바로 반환
        }
    }
}
