using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class GlobalUI : UIBase
    {
        [SerializeField] private Button menuButton;

        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            menuButton.onClick.AddListener(OnclickMenuButton); // 메뉴 버튼 클릭 시 메뉴 패널 토글
            titleButton.onClick.AddListener(OnClickTitleButton);
            // exitButton.onClick.AddListener(OnClickQuitButton);
        }

        private void OnEnable() // 활성화 될때
        {
            menuPanel.SetActive(false); // 메뉴 패널 꺼두기
        }
        private void OnclickMenuButton() 
        {
            menuPanel.SetActive(!menuPanel.activeSelf); // 메뉴 버튼 클릭시 메뉴 패널 토글
        }
        private void OnClickTitleButton() 
        {
            Main.Singleton.ChangeScene(SceneType.Title); // 타이틀 버튼 클릭시 타이틀 씬으로 변경
        }

        public void SetMenuPanel(bool willOpen) // 외부에서 메뉴 패널 열고 닫기 제어, ESC 누르면 나오게 하고 싶어서 한번 해봄
        {
            menuPanel.SetActive(willOpen);
        }
    }
}
