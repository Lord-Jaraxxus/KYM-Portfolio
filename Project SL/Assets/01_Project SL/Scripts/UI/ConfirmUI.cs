using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{

    public class ConfirmUI : UIBase
    {
        public static void SetConfirmUI(
            string titleMsg, string contextMsg,
            System.Action callbackOK = null,
            System.Action callbackCancel = null)
        {
            var popup = UIManager.Singleton.GetUI<ConfirmUI>(UIList.ConfirmUI); // ConfirmUI 인스턴스 가져오기
            popup.Init(titleMsg, contextMsg, callbackOK, callbackCancel); // ConfirmUI 초기화
        }

        public override bool IsNeedCursorVisible => true; // 커서 표시 필요

        [SerializeField] TextMeshProUGUI text_Title; // 제목 텍스트
        [SerializeField] TextMeshProUGUI text_Message; // 메시지 텍스트

        [SerializeField] private Button button_OK; // 확인 버튼
        [SerializeField] private Button button_Cancel; // 취소 버튼

        public event System.Action OnClickOK; // 버튼 클릭 이벤트
        public event System.Action OnClickCancel; // 버튼 클릭 이벤트

        private void Awake()
        {
            button_OK.onClick.AddListener(OnClickOKButton); // 확인 버튼 클릭 이벤트 연결
            button_Cancel.onClick.AddListener(OnClickCancelButton); // 취소 버튼 클릭 이벤트 연결
        }

        private void Init(string title, string context, System.Action callbackOK = null, System.Action callbackCancel = null) 
        {
            text_Title.text = title; // 제목 설정
            text_Message.text = context; // 메시지 설정

            OnClickOK += callbackOK; // 확인 버튼 클릭 이벤트에 콜백 등록
            OnClickCancel += callbackCancel; // 취소 버튼 클릭 이벤트에 콜백 등록
        }

        private void OnClickOKButton()
        {
            OnClickOK?.Invoke(); // 확인 버튼 클릭 이벤트 호출
            CloseUI(); // UI 닫기
        }

        private void OnClickCancelButton()
        {
            OnClickCancel?.Invoke(); // 취소 버튼 클릭 이벤트 호출
            CloseUI(); // UI 닫기
        }

        void CloseUI() 
        {
            OnClickOK = null; // 확인 버튼 클릭 이벤트 초기화
            OnClickCancel = null; // 취소 버튼 클릭 이벤트 초기화
            UIManager.Hide<ConfirmUI>(UIList.ConfirmUI); // ConfirmUI 숨기기
        }

    }
}
