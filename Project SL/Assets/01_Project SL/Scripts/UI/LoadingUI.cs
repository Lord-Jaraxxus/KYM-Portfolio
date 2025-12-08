using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KYM
{
    public class LoadingUI : UIBase
    {
        [Header("Components")]
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Image loadingBar;
        [SerializeField] private TextMeshProUGUI loadingPercentText;

        [Header("Setting")]
        [SerializeField] private float fadeSpeed = 1f;

        private bool isFadingIn = false;
        private bool isFadingOut = false;
        private bool isTaskExecuted = false;

        private event System.Action OnLoadingTask = null;


        private void Awake()
        {
            SetLoadingProgress(0f);
        }

        private void Update()
        {
            if (isFadingIn) 
            {
                group.alpha += Time.unscaledDeltaTime * fadeSpeed;
                if (group.alpha >= 1f) // 페이드 인이 완료되었을 때
                {
                    group.alpha = 1f;
                    isFadingIn = false;

                    // 로딩 작업 실행
                    if (isTaskExecuted == false)
                    {
                        StartCoroutine(ExecuteTask());
                    }
                }
            }
            else if (isFadingOut) 
            {
                group.alpha -= Time.unscaledDeltaTime * fadeSpeed;
                if (group.alpha <= 0f)
                {
                    group.alpha = 0f;
                    isFadingOut = false;
                    UIManager.Hide<LoadingUI>(UIList.LoadingUI); // 로딩 UI 숨김
                }
            }
        }

        IEnumerator ExecuteTask()
        {
            loadingPanel.SetActive(true); // 로딩바 키기 

            yield return new WaitForEndOfFrame();

            isTaskExecuted = true;
            OnLoadingTask?.Invoke();
        }
        public IEnumerator WaitForFadeInComplete()
        {
            // 페이드 인이 진행 중일 때까지 기다림
            yield return new WaitUntil(() => isFadingIn == false);
        }

        public void ShowLoadingUI(System.Action task)
        {
            group.alpha = 0f; // 투명하게 만들기
            loadingPanel.SetActive(false); // 로딩 패널 비활성화
            isFadingIn = true;
            isTaskExecuted = false;
            isFadingOut = false;

            OnLoadingTask = task; // 작업 등록

            Debug.Log("왜 페이드인을 안하고 시작할꼬");
        }

        public void HideLoadingUI()
        {
            loadingPanel.SetActive(false); // 로딩바 끄기

            isFadingOut = true;
            isFadingIn = false;
        }

        public void SetLoadingProgress(float progress)
        {
            progress = Mathf.Clamp01(progress); // 안정장치?
            loadingBar.fillAmount = progress;    // 0에서 1사이의 비율만큼 로딩바 채우기
            loadingPercentText.text = $"{(int)(progress * 100f)}%"; // 퍼센트 텍스트 업데이트
        }

    }
}
