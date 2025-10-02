using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class UIManager : SingletonBase<UIManager>   // UIManager는 SingletonBase를 상속받아 싱글톤 패턴을 구현
    {
        private Transform panelRoot; // 패널 UI를 담을 루트 트랜스폼
        private Transform popupRoot; // 팝업 UI를 담을 루트 트랜스폼
        private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>(); // 패널 UI들을 저장할 딕셔너리
        private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>(); // 팝업 UI들을 저장할 딕셔너리

        public Camera UICamera { get; private set; } // UI 카메라 프로퍼티

        private List<UIList> autoHideExceptUIs = new List<UIList>() // 자동으로 숨기지 않을 UI 목록
        {
            UIList.LoadingUI,
        };


        public void Initialize()
        {
            if (panelRoot == null) // 패널 루트가 없으면 생성
            {
                GameObject goPanelRoot = new GameObject("Panel Root"); // 패널 루트 게임 오브젝트 생성
                panelRoot = goPanelRoot.transform; // 트랜스폼 설정
                panelRoot.parent = this.transform; // UIManager의 자식으로 설정
                panelRoot.localPosition = Vector3.zero; // 위치 초기화
                panelRoot.localRotation = Quaternion.identity; // 회전 초기화
                panelRoot.localScale = Vector3.one; // 스케일 초기화
            }

            if (popupRoot == null) // 팝업 루트가 없으면 생성
            {
                GameObject goPopupRoot = new GameObject("Popup Root"); // 팝업 루트 게임 오브젝트 생성
                popupRoot = goPopupRoot.transform; // 트랜스폼 설정
                popupRoot.parent = this.transform; // UIManager의 자식으로 설정
                popupRoot.localPosition = Vector3.zero; // 위치 초기화
                popupRoot.localRotation = Quaternion.identity; // 회전 초기화
                popupRoot.localScale = Vector3.one; // 스케일 초기화
            }

            for (int index = (int)UIList.PANEL_START + 1; index < (int)UIList.PANEL_END; index++)
            {
                panels.Add((UIList)index, null); // 패널 딕셔너리에 UI 추가
            }
        
            for (int index = (int)UIList.POPUP_START + 1; index < (int)UIList.POPUP_END; index++)
            {
                popups.Add((UIList)index, null); // 팝업 딕셔너리에 UI 추가
            }

            if (!UICamera) // UI 카메라가 없으면 생성
            {
                GameObject newUICameraGo = new GameObject("UI Camera"); // UI 카메라 게임 오브젝트 생성
                newUICameraGo.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); // 위치와 회전 초기화
                UICamera = newUICameraGo.AddComponent<Camera>(); // 카메라 컴포넌트 추가
                UICamera.clearFlags = CameraClearFlags.Depth; // 카메라 설정
                UICamera.cullingMask = LayerMask.GetMask("UI"); // UI 레이어만 렌더링
                UICamera.fieldOfView = 60f;
                UICamera.nearClipPlane = 0.3f;
                UICamera.farClipPlane = 1000f;
                UICamera.orthographic = false;
                UICamera.depth = 10; // 다른 카메라보다 위에 렌더링 되도록 설정
            }
        }

        public static T Show<T>(UIList uiName) where T : UIBase 
        {
            var newUI = Singleton.GetUI<T>(uiName); // UIManger 싱글톤에서 해당 UI를 가져옴
            if (newUI == null) return null; // UI가 없으면 null 반환

            newUI.Show(); // UI를 보여줌
            return newUI; // 보여준 UI 반환
        }

        public static T Hide<T>(UIList uiName) where T : UIBase 
        {
            var targetUI = Singleton.GetUI<T>(uiName); // UIManager에서 UI를 가져옴
            if (!targetUI) return null; // UI가 없으면 null 반환

            targetUI.Hide(); // UI를 숨김
            return targetUI;
        }

        public T GetUI<T>(UIList uiName) where T : UIBase 
        {
            // UI가 팝업인지 패널인지에 따라 컨테이너 선택
            Dictionary<UIList, UIBase> container = 
                uiName > UIList.POPUP_START  &&  uiName < UIList.POPUP_END 
                ? popups : panels;

            // 팝업 루트 또는 패널 루트 설정
            Transform root =
                uiName > UIList.POPUP_START  &&  uiName < UIList.POPUP_END 
                ? popupRoot : panelRoot;

            if (!container.ContainsKey(uiName)) return null; // 컨테이너에 UI가 없으면 null 반환

            if (container[uiName] == null) // 키는 있는데 값이 null이면
            { 
                string path = $"UI/Prefabs/UI.{uiName}"; // UI 프리팹 경로 설정
                GameObject uiPrefab = Resources.Load<GameObject>(path); // 프리팹 로드

                if (uiPrefab == null) return null; // 프리팹이 없으면 null 반환

                var component = Instantiate(uiPrefab, root).GetComponent<T>(); // 프리팹 인스턴스화 & 루트의 자식으로 설정 + 컴포넌트 가져오기
                container[uiName] = component; // 컨테이너에 컴포넌트 저장

                if (container[uiName]) // 컴포넌트가 있으면
                {
                    container[uiName].gameObject.SetActive(false); // UI를 비활성화 상태로 설정
                }

                if (container[uiName].TryGetComponent(out UIBase uiBase)) // UIBase 컴포넌트가 있으면
                {
                    if(uiBase.IsDepthUI) // Depth UI라면
                    {
                        Canvas canvas = uiBase.GetComponent<Canvas>(); // Canvas 컴포넌트 가져오기
                        canvas.renderMode = RenderMode.ScreenSpaceCamera; // 캔버스 렌더 모드를 ScreenSpaceCamera로 설정
                        canvas.worldCamera = this.UICamera; // 메인 카메라를 설정
                    }
                }
            }
            return (T)container[uiName]; // 컨테이너에서 UI 반환
        }

        public void HideAll() 
        {
            HideAllPanel(); // 모든 패널 숨기기
            HideAllPopup(); // 모든 팝업 숨기기
        }
        
        public void HideAllPanel() 
        {
            foreach (var panel in panels) // 모든 패널에 대해 반복
            {
                if (autoHideExceptUIs.Contains(panel.Key)) continue; // 자동 숨기기 제외 목록에 있으면 건너뜀

                if ((panel.Value != null))  // 패널이 null이 아니면
                {
                    panel.Value.Hide(); // 패널 숨기기
                }
            }
        }

        public void HideAllPopup() 
        {
            foreach (var popup in popups)
            {
                if (autoHideExceptUIs.Contains(popup.Key))
                    continue; // 다음 팝업으로 넘어감

                if (popup.Value != null)
                {
                    popup.Value.Hide(); // 팝업 숨김
                }
            }
        }



    }
}
