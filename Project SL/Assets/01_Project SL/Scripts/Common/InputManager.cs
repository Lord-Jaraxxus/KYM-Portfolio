using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class InputManager : SingletonBase<InputManager>
    {
        public Vector2 InputMove { get; private set; }  // 이동 입력 벡터 (수평, 수직)
        public Vector2 InputLook { get; private set; }  // 마우스 이동 입력 벡터 (수평, 수직)
        public float inputMouseScroll { get; private set; } // 마우스 스크롤 입력 값

        public event System.Action OnInputLmc; // 좌클릭 입력 이벤트
        public event System.Action onInputRmc; // 우클릭 입력 이벤트 
        public event System.Action onInputSpace; // 스페이스바 입력 이벤트
        public event System.Action onInputShift; // 쉬프트키 입력 이벤트
        public event System.Action onInputCtrl; // 컨트롤키 입력 이벤트
        public event System.Action onInputF; // F키 입력 이벤트

        public event System.Action<float> onInputMouseScroll; // 마우스 스크롤 입력 이벤트

        public bool IsForceCursorVisible
        {
            get => IsForceCursorVisible;
            set
            {
                IsForceCursorVisible = value; // 커서 강제 표시 여부 설정
                if (IsForceCursorVisible)
                {
                    SetCursorVisible(true); // 커서 강제 표시가 true면 커서 보이기
                }
                else
                {
                    SetCursorVisible(false); // 커서 강제 표시가 false면 커서 숨김
                }
            }
        }
        private bool isForceCursorVisible = false; // 커서 강제 표시 여부 (기본값은 false)

        public void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.visible = true;  // 커서 보이기
                Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            }
            else
            {
                Cursor.visible = false; // 커서 숨김
                Cursor.lockState = CursorLockMode.Locked; // 커서 잠금 상태 설정
            }
        }

        private void Update()
        {
            if (!isForceCursorVisible)
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    SetCursorVisible(true); // Alt 키를 누르고 있으면 커서 보이기
                }
                else
                {
                    SetCursorVisible(false); // Alt 키를 떼면 커서 숨김
                }
            }

            InputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // 이동 입력 벡터 설정
            InputLook = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // 마우스 이동 입력 벡터 설정
            inputMouseScroll = Input.GetAxis("Mouse ScrollWheel"); // 마우스 스크롤 입력 값 설정

            if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭이 눌러지면 true
            {
                OnInputLmc?.Invoke(); // 좌클릭 입력 이벤트 발생
            }

            if (Input.GetMouseButtonDown(1))  // 마우스 우클릭이 눌러지면 true
            {
                onInputRmc?.Invoke(); // 우클릭 입력 이벤트 발생
            }
            if( Input.GetKeyDown(KeyCode.Space)) // 스페이스바가 눌러지면 true
            {
                onInputSpace?.Invoke(); // 스페이스바 입력 이벤트 발생
            }
            if( Input.GetKeyDown(KeyCode.LeftShift)) // 쉬프트키가 눌러지면 true
            {
                onInputShift?.Invoke(); // 쉬프트키 입력 이벤트 발생
            }
            if( Input.GetKeyDown(KeyCode.LeftControl)) // 컨트롤키가 눌러지면 true
            {
                onInputCtrl?.Invoke(); // 컨트롤키 입력 이벤트 발생
            }
            if( Input.GetKeyDown(KeyCode.F)) // F키가 눌러지면 true
            {
                onInputF?.Invoke(); // F키 입력 이벤트 발생
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                onInputMouseScroll?.Invoke(scroll);
            }
        }

    }
}
