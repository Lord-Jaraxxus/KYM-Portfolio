using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class TitleUI : UIBase
    {
        public override bool IsNeedCursorVisible => true;

        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnClickStartButton);
            quitButton.onClick.AddListener(OnClickQuitButton);
        }


        public void OnClickStartButton()
        {
            // Main.Singleton.ChangeScene(SceneType.Ingame); // 게임 시작 버튼 클릭 시 Ingame 씬으로 변경 <- 이게 기존 코드였고
            // Main.Singleton.ChangeScene(SceneType.Town); // 게임 시작 버튼 클릭 시 Town 씬으로 변경 <- 이러면 아예 씬을 다시 로드해버림; 로딩도 나오고..

            UIManager.Show<PlayerHUD>(UIList.PlayerHUD); // Player HUD UI 표시
            UIManager.Show<GlobalUI>(UIList.GlobalUI); // Global UI 표시
            CameraSystem.Instance.SetActiveTitleCamera(false); // 타이틀 카메라 비활성화 (게임 시작 시 타이틀 카메라 끄기)
            UIManager.Hide<TitleUI>(UIList.TitleUI); // 타이틀 UI 숨김 (게임 시작 시 타이틀 UI 끄기)
        }

        public void OnClickQuitButton()
        {
            Main.Singleton.SystemQuit(); // 게임 종료 버튼 클릭 시 게임 종료
        }
    }
}
