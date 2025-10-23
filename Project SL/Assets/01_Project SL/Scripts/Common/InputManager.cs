using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class InputManager : MonoBehaviour
    {
        public Vector2 InputMove { get; private set; }  // �̵� �Է� ���� (����, ����)
        public Vector2 InputLook { get; private set; }  // ���콺 �̵� �Է� ���� (����, ����)
        public float inputMouseScroll { get; private set; } // ���콺 ��ũ�� �Է� ��

        public event System.Action OnInputLmc; // ��Ŭ�� �Է� �̺�Ʈ
        public event System.Action onInputRmc; // ��Ŭ�� �Է� �̺�Ʈ 
        public event System.Action onInputSpace; // �����̽��� �Է� �̺�Ʈ
        public event System.Action onInputShift; // ����ƮŰ �Է� �̺�Ʈ
        public event System.Action onInputCtrl; // ��Ʈ��Ű �Է� �̺�Ʈ

        public event System.Action<float> onInputMouseScroll; // ���콺 ��ũ�� �Է� �̺�Ʈ

        public bool IsForceCursorVisible
        {
            get => IsForceCursorVisible;
            set
            {
                IsForceCursorVisible = value; // Ŀ�� ���� ǥ�� ���� ����
                if (IsForceCursorVisible)
                {
                    SetCursorVisible(true); // Ŀ�� ���� ǥ�ð� true�� Ŀ�� ���̱�
                }
                else
                {
                    SetCursorVisible(false); // Ŀ�� ���� ǥ�ð� false�� Ŀ�� ����
                }
            }
        }
        private bool isForceCursorVisible = false; // Ŀ�� ���� ǥ�� ���� (�⺻���� false)

        public void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.visible = true;  // Ŀ�� ���̱�
                Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
            }
            else
            {
                Cursor.visible = false; // Ŀ�� ����
                Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ��� ���� ����
            }
        }

        private void Update()
        {
            if (!isForceCursorVisible)
            {
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    SetCursorVisible(true); // Alt Ű�� ������ ������ Ŀ�� ���̱�
                }
                else
                {
                    SetCursorVisible(false); // Alt Ű�� ���� Ŀ�� ����
                }
            }

            InputMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // �̵� �Է� ���� ����
            InputLook = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // ���콺 �̵� �Է� ���� ����
            inputMouseScroll = Input.GetAxis("Mouse ScrollWheel"); // ���콺 ��ũ�� �Է� �� ����

            if (Input.GetMouseButton(0)) // ���콺 ��Ŭ���� ������������ ��� true
            {
                OnInputLmc?.Invoke(); // ��Ŭ�� �Է� �̺�Ʈ �߻�
            }

            if (Input.GetMouseButtonDown(1))  // ���콺 ��Ŭ���� ������������ ��� true
            {
                onInputRmc?.Invoke(); // ��Ŭ�� �Է� �̺�Ʈ �߻�
            }
            if( Input.GetKeyDown(KeyCode.Space)) // �����̽��ٰ� �������� true
            {
                onInputSpace?.Invoke(); // �����̽��� �Է� �̺�Ʈ �߻�
            }
            if( Input.GetKeyDown(KeyCode.LeftShift)) // ����ƮŰ�� �������� true
            {
                onInputShift?.Invoke(); // ����ƮŰ �Է� �̺�Ʈ �߻�
            }
            if( Input.GetKeyDown(KeyCode.LeftControl)) // ��Ʈ��Ű�� �������� true
            {
                onInputCtrl?.Invoke(); // ��Ʈ��Ű �Է� �̺�Ʈ �߻�
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                onInputMouseScroll?.Invoke(scroll);
            }
        }

    }
}
