using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        [SerializeField] TextMeshProUGUI attackText;
        [SerializeField] TextMeshProUGUI defenseText;

        // 경험치바 애니메이션을 위한 변수들
        private int levelUpCount = 0;
        private int lastLevel = 0;
        private int lastExp = 0;
        private int lastReqExp = 0;
        private int animationTargetExp = 0;
        private float expBarRatio = 0f;

        //private void Start()
        //{
        //    Initialize();
        //}

        private void Update()
        {
            // 전달된 레벨업카운트만큼 레벨업 애니메이션 처리
            if (levelUpCount > 0)
            {
                if (lastExp < lastReqExp)
                {
                    lastExp++;
                    expBarRatio = (float)lastExp / lastReqExp;
                    expBar.fillAmount = Mathf.Min(expBarRatio, 1f);
                    expText.text = $"{lastExp} / {lastReqExp}";
                }
                else // 경험치바가 가득 찼다면 레벨업 처리
                {
                    lastLevel++;
                    levelUpCount--;
                    expBarRatio = 0f;
                    expBar.fillAmount = 0f;

                    lastExp = 0;
                    lastReqExp = UserDataModel.Singleton.CalculateRequiredExp(lastLevel);

                    levelText.text = $"Lv.{lastLevel}"; // 레벨 UI 갱신
                }
            }
            else if (lastExp < animationTargetExp) // 레벨업 애니메이션이 끝났거나 없고, 경험치 애니메이션 처리가 남았다면
            {
                lastExp++;
                expBarRatio = (float)lastExp / lastReqExp;
                expBar.fillAmount = Mathf.Min(expBarRatio, 1f);
                expText.text = $"{lastExp} / {lastReqExp}";
            }
        }

        private void OnEnable()
        {
            UserDataModel.Singleton.OnExpUpdated += RefreshExpUI;
            UserDataModel.Singleton.OnLevelUpdated += RefreshLevelUI;

            // 여기서 뭔가... 뭔가.. last들이랑 현재 정보들을 가지고 뭔가를 해야 할 것 같은데..
            var playerInfo = UserDataModel.Singleton.PlayerInfoDto;
            RefreshExpUI(playerInfo.CurrentExp, playerInfo.RequiredExp);
            RefreshLevelUI(playerInfo.Level, playerInfo.Level - lastLevel);
        }

        private void OnDisable()
        {
            UserDataModel.Singleton.OnExpUpdated -= RefreshExpUI;
            UserDataModel.Singleton.OnLevelUpdated -= RefreshLevelUI;
        }


        public void Initialize()
        {
            var playerInfo = UserDataModel.Singleton.PlayerInfoDto;

            lastLevel = playerInfo.Level;
            lastExp = playerInfo.CurrentExp;
            lastReqExp = playerInfo.RequiredExp;

            levelText.text = $"Lv.{playerInfo.Level}";
            float expRatio = (float)playerInfo.CurrentExp / playerInfo.RequiredExp;
            expBar.fillAmount = expRatio;
            expText.text = $"{playerInfo.CurrentExp} / {playerInfo.RequiredExp}";
        }

        public void RefreshExpUI(int curExp, int reqExp)
        {
            animationTargetExp = curExp;

            //float expRatio = (float)curExp / reqExp;
            //expBar.fillAmount = expRatio;
            //expText.text = $"{curExp} / {reqExp}";
        }

        public void RefreshLevelUI(int level, int levelUpCount)
        {
            // levelText.text = $"Lv.{level}";
            this.levelUpCount = level - lastLevel;
        }

    }
}
