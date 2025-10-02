using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class TitleUI : UIBase
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            startButton.onClick.AddListener(OnClickStartButton);
            quitButton.onClick.AddListener(OnClickQuitButton);
        }


        public void OnClickStartButton()
        {
            Main.Singleton.ChangeScene(SceneType.Ingame); // ���� ���� ��ư Ŭ�� �� Ingame ������ ����
        }
        public void OnClickQuitButton()
        {
            Main.Singleton.SystemQuit(); // ���� ���� ��ư Ŭ�� �� ���� ����
        }
    }
}
