using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class LoadingUI : UIBase
    {
        [Header("Components")]
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private UnityEngine.UI.Image loadingBar;

        [Header("Setting")]
        [SerializeField] private float fadeSpeed = 1f;

        private bool isShowing = false;
        private bool isHiding = false;
        private bool isTaskExecuted = false;

        private event System.Action OnLoadingTask = null;


        private void Update()
        {
            if (isShowing) 
            {
                group.alpha += Time.unscaledDeltaTime * fadeSpeed;
                if (group.alpha >= 1f) // 페이드 인이 완료되었을 때
                {
                    group.alpha = 1f;
                    isShowing = false;

                    // 로딩 작업 실행
                    if (!isTaskExecuted)
                    {
                        StartCoroutine(ExecuteTask());
                    }
                }
            }
            else if (isHiding) 
            {
                group.alpha -= Time.unscaledDeltaTime * fadeSpeed;
                if (group.alpha <= 0f)
                {
                    group.alpha = 0f;
                    isHiding = false;
                    UIManager.Hide<LoadingUI>(UIList.LoadingUI); // 로딩 UI 숨김
                }
            }
        }

        IEnumerator ExecuteTask()
        {
            loadingPanel.SetActive(true); // 로딩 패널 활성화

            yield return new WaitForEndOfFrame();

            isTaskExecuted = true;
            OnLoadingTask?.Invoke();

            HideLoadingUI();
        }

        public void ShowLoadingUI(System.Action task)
        {
            group.alpha = 0f; // 투명하게 만들기
            loadingPanel.SetActive(false); // 로딩 패널 비활성화
            isShowing = true;
            isTaskExecuted = false;
            isHiding = false;

            OnLoadingTask = task; // 로딩 작업 등록
        }

        public void HideLoadingUI()
        {
            isHiding = true;
            isShowing = false;
        }

        public void SetLoadingProgress(float progress)
        {
            progress = Mathf.Clamp01(progress); // 안정장치?
           loadingBar.fillAmount = progress;    // 0에서 1사이의 비율만큼 로딩바 채우기
        }

    }
}
