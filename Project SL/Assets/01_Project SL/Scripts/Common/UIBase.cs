using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public abstract class UIBase : MonoBehaviour    // �߻� Ŭ����, �θ�θ� ����
    {
        public virtual bool IsDepthUI { get; } = false; // UI�� Depth UI���� ���� (�⺻���� false)


        public virtual void Show() // UI�� �����ִ� �޼���
        {
            gameObject.SetActive(true); // ���� ������Ʈ Ȱ��ȭ
        }

        public virtual void Hide() // UI�� ����� �޼���
        {
            gameObject.SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
        }
    }
}
