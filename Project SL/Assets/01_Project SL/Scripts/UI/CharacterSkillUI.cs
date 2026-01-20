using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class CharacterSkillUI : UIBase
    {
        // 스킬 포인트 텍스트
        [SerializeField] private TextMeshProUGUI skillPoint;

        // 스킬 아이콘들
        [SerializeField] private Image icon_Skill1;
        [SerializeField] private Image icon_Skill2;
        [SerializeField] private Image icon_Skill3;
        [SerializeField] private Image icon_Skill4;

        // 스킬 이름들
        [SerializeField] private TextMeshProUGUI name_Skill1;
        [SerializeField] private TextMeshProUGUI name_Skill2;
        [SerializeField] private TextMeshProUGUI name_Skill3;
        [SerializeField] private TextMeshProUGUI name_Skill4;

        // 스킬 레벨들
        [SerializeField] private TextMeshProUGUI level_Skill1;
        [SerializeField] private TextMeshProUGUI level_Skill2;
        [SerializeField] private TextMeshProUGUI level_Skill3;
        [SerializeField] private TextMeshProUGUI level_Skill4;

        // 스킬 레벨업 버튼들
        [SerializeField] private Button upButton_Skill1;
        [SerializeField] private Button upButton_Skill2;
        [SerializeField] private Button upButton_Skill3;
        [SerializeField] private Button upButton_Skill4;


        private void Awake()
        {
            upButton_Skill1.onClick.AddListener(OnClickUpButton_Skill1);
            upButton_Skill2.onClick.AddListener(OnClickUpButton_Skill2);
            upButton_Skill3.onClick.AddListener(OnClickUpButton_Skill3);
            upButton_Skill4.onClick.AddListener(OnClickUpButton_Skill4);

            Initialize();
        }
        private void OnEnable()
        {
            UserDataModel.Singleton.OnLevelUpdated += OnReceiveLevelUpdated;
            RefreshSkillPoints(); // 스킬 포인트 갱신
        }
        private void OnDisable()
        {
            UserDataModel.Singleton.OnLevelUpdated -= OnReceiveLevelUpdated;
        }

        private void OnClickUpButton_Skill1() => OnClickUpButton(0);
        private void OnClickUpButton_Skill2() => OnClickUpButton(1);
        private void OnClickUpButton_Skill3() => OnClickUpButton(2);
        private void OnClickUpButton_Skill4() => OnClickUpButton(3);

        private void Initialize()
        {
            int skillIndex = 0;

            // 유저데이터에 로딩해놓은 스킬 정보들을 순서대로 가져와서 초기화
            foreach (var skillData in UserDataModel.Singleton.PlayerSkillDto.PlayerSkills)
            {
                SkillDataSO skillDataSO = GameDataModel.Singleton.SkillDatabase.SkillDatas[skillData.SkillID];

                switch (skillIndex)
                {
                    case 0:
                        icon_Skill1.sprite = skillDataSO.SkillIcon;
                        name_Skill1.text = skillDataSO.SkillName;
                        level_Skill1.text = "Lv." + skillData.SkillLevel.ToString();
                        break;
                    case 1:
                        icon_Skill2.sprite = skillDataSO.SkillIcon;
                        name_Skill2.text = skillDataSO.SkillName;
                        level_Skill2.text = "Lv." + skillData.SkillLevel.ToString();
                        break;
                    case 2:
                        icon_Skill3.sprite = skillDataSO.SkillIcon;
                        name_Skill3.text = skillDataSO.SkillName;
                        level_Skill3.text = "Lv." + skillData.SkillLevel.ToString();
                        break;
                    case 3:
                        icon_Skill4.sprite = skillDataSO.SkillIcon;
                        name_Skill4.text = skillDataSO.SkillName;
                        level_Skill4.text = "Lv." + skillData.SkillLevel.ToString();
                        break;
                }
                skillIndex++;
            }

            RefreshSkillPoints();
        }

        private void OnReceiveLevelUpdated(int level, int levelUpCount)
        {
            RefreshSkillPoints();
        }

        private void RefreshSkillPoints()
        {
            skillPoint.text = "Skill Points : " + UserDataModel.Singleton.PlayerSkillDto.SkillPoints.ToString();
        }

        private void RefreshSkillSlot(int skillIndex) 
        {
            string skillID = UserDataModel.Singleton.PlayerSkillDto.PlayerSkills[skillIndex].SkillID;
            int skillLevel = UserDataModel.Singleton.GetSkillLevel(skillID);

            switch (skillIndex)
            {
                case 0:
                    level_Skill1.text = "Lv." + skillLevel.ToString();
                    break;
                case 1:
                    level_Skill2.text = "Lv." + skillLevel.ToString();
                    break;
                case 2:
                    level_Skill3.text = "Lv." + skillLevel.ToString();
                    break;
                case 3:
                    level_Skill4.text = "Lv." + skillLevel.ToString();
                    break;
            }
        }

        private void OnClickUpButton(int skillIndex)
        {
            var playerSkillData = UserDataModel.Singleton.PlayerSkillDto.PlayerSkills[skillIndex];

            if (UserDataModel.Singleton.PlayerSkillDto.SkillPoints > 0)
            {
                // 스킬 포인트 차감
                UserDataModel.Singleton.PlayerSkillDto.TrySpendSkillPoint(1);
                // 스킬 레벨업
                UserDataModel.Singleton.PlayerSkillDto.PlayerSkills[skillIndex].SkillLevel += 1;
                // UI 갱신
                RefreshSkillSlot(skillIndex);
                RefreshSkillPoints();
            }
            else
            {
                Debug.Log("스킬 포인트가 부족합니다.");
            }
        }

    }
}
