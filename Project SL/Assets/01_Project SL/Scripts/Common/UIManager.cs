using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class UIManager : SingletonBase<UIManager>   // UIManager�� SingletonBase�� ��ӹ޾� �̱��� ������ ����
    {
        private Transform panelRoot; // �г� UI�� ���� ��Ʈ Ʈ������
        private Transform popupRoot; // �˾� UI�� ���� ��Ʈ Ʈ������
        private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>(); // �г� UI���� ������ ��ųʸ�
        private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>(); // �˾� UI���� ������ ��ųʸ�

        public Camera UICamera { get; private set; } // UI ī�޶� ������Ƽ

        private List<UIList> autoHideExceptUIs = new List<UIList>() // �ڵ����� ������ ���� UI ���
        {
            UIList.LoadingUI,
        };


        public void Initialize()
        {
            if (panelRoot == null) // �г� ��Ʈ�� ������ ����
            {
                GameObject goPanelRoot = new GameObject("Panel Root"); // �г� ��Ʈ ���� ������Ʈ ����
                panelRoot = goPanelRoot.transform; // Ʈ������ ����
                panelRoot.parent = this.transform; // UIManager�� �ڽ����� ����
                panelRoot.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                panelRoot.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ
                panelRoot.localScale = Vector3.one; // ������ �ʱ�ȭ
            }

            if (popupRoot == null) // �˾� ��Ʈ�� ������ ����
            {
                GameObject goPopupRoot = new GameObject("Popup Root"); // �˾� ��Ʈ ���� ������Ʈ ����
                popupRoot = goPopupRoot.transform; // Ʈ������ ����
                popupRoot.parent = this.transform; // UIManager�� �ڽ����� ����
                popupRoot.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                popupRoot.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ
                popupRoot.localScale = Vector3.one; // ������ �ʱ�ȭ
            }

            for (int index = (int)UIList.PANEL_START + 1; index < (int)UIList.PANEL_END; index++)
            {
                panels.Add((UIList)index, null); // �г� ��ųʸ��� UI �߰�
            }
        
            for (int index = (int)UIList.POPUP_START + 1; index < (int)UIList.POPUP_END; index++)
            {
                popups.Add((UIList)index, null); // �˾� ��ųʸ��� UI �߰�
            }

            if (!UICamera) // UI ī�޶� ������ ����
            {
                GameObject newUICameraGo = new GameObject("UI Camera"); // UI ī�޶� ���� ������Ʈ ����
                newUICameraGo.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); // ��ġ�� ȸ�� �ʱ�ȭ
                UICamera = newUICameraGo.AddComponent<Camera>(); // ī�޶� ������Ʈ �߰�
                UICamera.clearFlags = CameraClearFlags.Depth; // ī�޶� ����
                UICamera.cullingMask = LayerMask.GetMask("UI"); // UI ���̾ ������
                UICamera.fieldOfView = 60f;
                UICamera.nearClipPlane = 0.3f;
                UICamera.farClipPlane = 1000f;
                UICamera.orthographic = false;
                UICamera.depth = 10; // �ٸ� ī�޶󺸴� ���� ������ �ǵ��� ����
            }
        }

        public static T Show<T>(UIList uiName) where T : UIBase 
        {
            var newUI = Singleton.GetUI<T>(uiName); // UIManger �̱��濡�� �ش� UI�� ������
            if (newUI == null) return null; // UI�� ������ null ��ȯ

            newUI.Show(); // UI�� ������
            return newUI; // ������ UI ��ȯ
        }

        public static T Hide<T>(UIList uiName) where T : UIBase 
        {
            var targetUI = Singleton.GetUI<T>(uiName); // UIManager���� UI�� ������
            if (!targetUI) return null; // UI�� ������ null ��ȯ

            targetUI.Hide(); // UI�� ����
            return targetUI;
        }

        public T GetUI<T>(UIList uiName) where T : UIBase 
        {
            // UI�� �˾����� �г������� ���� �����̳� ����
            Dictionary<UIList, UIBase> container = 
                uiName > UIList.POPUP_START  &&  uiName < UIList.POPUP_END 
                ? popups : panels;

            // �˾� ��Ʈ �Ǵ� �г� ��Ʈ ����
            Transform root =
                uiName > UIList.POPUP_START  &&  uiName < UIList.POPUP_END 
                ? popupRoot : panelRoot;

            if (!container.ContainsKey(uiName)) return null; // �����̳ʿ� UI�� ������ null ��ȯ

            if (container[uiName] == null) // Ű�� �ִµ� ���� null�̸�
            { 
                string path = $"UI/Prefabs/UI.{uiName}"; // UI ������ ��� ����
                GameObject uiPrefab = Resources.Load<GameObject>(path); // ������ �ε�

                if (uiPrefab == null) return null; // �������� ������ null ��ȯ

                var component = Instantiate(uiPrefab, root).GetComponent<T>(); // ������ �ν��Ͻ�ȭ & ��Ʈ�� �ڽ����� ���� + ������Ʈ ��������
                container[uiName] = component; // �����̳ʿ� ������Ʈ ����

                if (container[uiName]) // ������Ʈ�� ������
                {
                    container[uiName].gameObject.SetActive(false); // UI�� ��Ȱ��ȭ ���·� ����
                }

                if (container[uiName].TryGetComponent(out UIBase uiBase)) // UIBase ������Ʈ�� ������
                {
                    if(uiBase.IsDepthUI) // Depth UI���
                    {
                        Canvas canvas = uiBase.GetComponent<Canvas>(); // Canvas ������Ʈ ��������
                        canvas.renderMode = RenderMode.ScreenSpaceCamera; // ĵ���� ���� ��带 ScreenSpaceCamera�� ����
                        canvas.worldCamera = this.UICamera; // ���� ī�޶� ����
                    }
                }
            }
            return (T)container[uiName]; // �����̳ʿ��� UI ��ȯ
        }

        public void HideAll() 
        {
            HideAllPanel(); // ��� �г� �����
            HideAllPopup(); // ��� �˾� �����
        }
        
        public void HideAllPanel() 
        {
            foreach (var panel in panels) // ��� �гο� ���� �ݺ�
            {
                if (autoHideExceptUIs.Contains(panel.Key)) continue; // �ڵ� ����� ���� ��Ͽ� ������ �ǳʶ�

                if ((panel.Value != null))  // �г��� null�� �ƴϸ�
                {
                    panel.Value.Hide(); // �г� �����
                }
            }
        }

        public void HideAllPopup() 
        {
            foreach (var popup in popups)
            {
                if (autoHideExceptUIs.Contains(popup.Key))
                    continue; // ���� �˾����� �Ѿ

                if (popup.Value != null)
                {
                    popup.Value.Hide(); // �˾� ����
                }
            }
        }



    }
}
