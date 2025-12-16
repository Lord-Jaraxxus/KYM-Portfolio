using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class PlayerHUD : UIBase
    {
        [SerializeField] Image hpBar;
        [SerializeField] Image spBar;

        [SerializeField] TextMeshProUGUI hpText;
        [SerializeField] TextMeshProUGUI spText;

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
    }
}
