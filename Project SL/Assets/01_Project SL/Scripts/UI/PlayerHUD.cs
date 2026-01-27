using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class PlayerHUD : UIBase
    {
        // Hp, Sp UI
        [SerializeField] Image hpBar;
        [SerializeField] Image spBar;
        [SerializeField] TextMeshProUGUI hpText;
        [SerializeField] TextMeshProUGUI spText;

        // Gold UI
        [SerializeField] TextMeshProUGUI goldText;

        // 숏컷 UI
        [SerializeField] Button button_I;
        [SerializeField] Button button_K;
        [SerializeField] Button button_P;
        [SerializeField] Button button_U;

        private void Awake()
        {
            button_I.onClick.AddListener(OnClickIButton);
            button_K.onClick.AddListener(OnClickKButton);
            button_P.onClick.AddListener(OnClickPButton);
            button_U.onClick.AddListener(OnClickUButton);
        }

        public void RefreshHpUI(float currentHp, float maxHp)
        {
            float hpRatio = currentHp / maxHp;
            hpBar.fillAmount = hpRatio;
            hpText.text = $"{currentHp} / {maxHp}";
        }

        public void RefreshSpUI(float currentSp, float maxSp)
        {
            float spRatio = currentSp / maxSp;
            spBar.fillAmount = spRatio;
            spText.text = $"{currentSp} / {maxSp}";
        }

        public void RefreshGoldUI(int currentGold)
        {
            goldText.text = $"{currentGold}";
        }


        // 숏컷 버튼들 메서드들
        private void OnClickIButton() // 인벤토리 UI 토글
        {
            var inventoryUI = UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);

            if (inventoryUI.gameObject.activeSelf)
            {
                UIManager.Hide<InventoryUI>(UIList.InventoryUI);
            }
            else
            {
                UIManager.Show<InventoryUI>(UIList.InventoryUI);
            }
        }
        private void OnClickKButton() // 스킬창 UI 토글
        {
            var characterSkillUI = UIManager.Singleton.GetUI<CharacterSkillUI>(UIList.CharacterSkillUI);

            if (characterSkillUI.gameObject.activeSelf)
            {
                UIManager.Hide<CharacterSkillUI>(UIList.CharacterSkillUI);
            }
            else
            {
                UIManager.Show<CharacterSkillUI>(UIList.CharacterSkillUI);
            }
        }
        private void OnClickPButton() // 장비창 UI 토글
        {
            var characterEquipUI = UIManager.Singleton.GetUI<CharacterEquipUI>(UIList.CharacterEquipUI);

            if (characterEquipUI.gameObject.activeSelf)
            {
                UIManager.Hide<CharacterEquipUI>(UIList.CharacterEquipUI);
            }
            else
            {
                UIManager.Show<CharacterEquipUI>(UIList.CharacterEquipUI);
            }
        }
        private void OnClickUButton() // 캐릭터 정보창 UI 토글
        {
            var CharacterInfoUI = UIManager.Singleton.GetUI<CharacterInfoUI>(UIList.CharacterInfoUI);

            if (CharacterInfoUI.gameObject.activeSelf)
            {
                UIManager.Hide<CharacterInfoUI>(UIList.CharacterInfoUI);
            }
            else
            {
                UIManager.Show<CharacterInfoUI>(UIList.CharacterInfoUI);
            }
        }
    }
}
