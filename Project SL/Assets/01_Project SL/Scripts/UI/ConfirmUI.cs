using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public enum ConfirmType
    {
        Entrance, // 입장 (씬 전환)
    }

    public class ConfirmUI : UIBase
    {
        public override bool IsNeedCursorVisible => true; // 커서 표시 필요

        [SerializeField] TextMeshProUGUI text_Title; // 제목 텍스트
        [SerializeField] TextMeshProUGUI text_Message; // 메시지 텍스트

        [SerializeField] private Button button_OK; // 확인 버튼
        [SerializeField] private Button button_Cancel; // 취소 버튼

        ConfirmType currentType; // 현재 확인 UI 타입
        public event System.Action <bool> OnClickButton; // 버튼 클릭 이벤트

        private void Awake()
        {
            button_OK.onClick.AddListener(OnClickOKButton); // 확인 버튼 클릭 이벤트 연결
            button_Cancel.onClick.AddListener(OnClickCancelButton); // 취소 버튼 클릭 이벤트 연결
        }

        private void OnClickOKButton()
        {
            OnClickButton?.Invoke(true); // 확인 버튼 클릭 이벤트 호출
        }

        private void OnClickCancelButton()
        {
            OnClickButton?.Invoke(false); // 취소 버튼 클릭 이벤트 호출
        }

        public void SetConfirmUI(ConfirmType type)
        {
            switch (type)
            {
                case ConfirmType.Entrance:
                    text_Title.text = "Dungeon Entrance"; // 제목 설정
                    text_Message.text = "Do you want to enter the dungeon?"; // 메시지 설정
                    currentType = ConfirmType.Entrance;
                    break;
                default:
                    break;
            }
        }
    }
}
