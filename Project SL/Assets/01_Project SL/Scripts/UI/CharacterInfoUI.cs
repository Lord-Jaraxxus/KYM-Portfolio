using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class CharacterInfoUI : UIBase
    {
        [SerializeField] Image expBar;

        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI expText;

        private void Start()
        {
    
        }

        private void OnEnable()
        {
            UserDataModel.Singleton.OnExpUpdated += RefreshExpUI;
            UserDataModel.Singleton.OnLevelUpdated += RefreshLevelUI;

            UserDataModel.Singleton.AddExp(0); // UI 초기화용 호출
        }
        private void OnDisable()
        {
            UserDataModel.Singleton.OnExpUpdated -= RefreshExpUI;
            UserDataModel.Singleton.OnLevelUpdated -= RefreshLevelUI;
        }

        public void RefreshExpUI(int curExp, int reqExp)
        {
            float expRatio = (float)curExp / reqExp;
            expBar.fillAmount = expRatio;
            Debug.Log(expRatio);
            expText.text = $"{curExp} / {reqExp}";
        }

        public void RefreshLevelUI(int level)
        {
            levelText.text = $"Lv.{level}";
        }

    }
}
