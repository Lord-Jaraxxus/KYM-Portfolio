using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public enum SceneType 
    {
        None,
        Title,  // 타이틀 씬
        Ingame, // 인게임 씬
    }

    public class Main : SingletonBase<Main>
    {
        private bool isInitialized = false;
        private SceneType currentSceneType = SceneType.None;

        private SceneBase sceneInstance; // 현재 씬 인스턴스를 저장하는 변수

        private void Start()
        {
            Initialize();   // 초기화 메서드 호출
        }

        private void Initialize()
        {
            if (isInitialized) return; // 이미 초기화된 경우 중복 실행 방지

            // var soundManagerPrefab = Resources.Load<GameObject>("Sound/Prefab/KYM.SoundManager"); // 사운드 매니저 프리팹 로드
            // var soundManagerInst = Instantiate(soundManagerPrefab); // 사운드 매니저 인스턴스 생성

            // UIManager.Singleton.Initialize(); // UIManager 초기화
            // GameDataModel.Singleton.Initialize(); // GameDataModel 초기화
            // UserDataModel.Singleton.Initialize(); // UserDataModel 초기화
#if UNITY_EDITOR
            UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            if (activeScene.name == "Main") 
            {
                ChangeScene(SceneType.Title); // 타이틀 씬으로 전환
            }
#else
            ChangeScene(SceneType.Title); 
#endif
            isInitialized = true; // 초기화 완료 플래그 설정
        }

        public void ChangeScene(SceneType sceneType, System.Action sceneLoadAfterCallback = null)   // 씬 변경 메서드 
        {
            if (currentSceneType == sceneType) return; // 현재 씬 타입과 변경하려는 씬 타입이 같으면 아무 작업도 하지 않음

            Time.timeScale = 1f; // 시간 스케일 초기화

            switch (sceneType)
            {
                case SceneType.Title:
                    ChangeScene<TitleScene>(sceneType, sceneLoadAfterCallback);
                    // SoundManager.StopAll(); // 모든 사운드 정지
                    // SoundManager.PlayBGM("BGM_Title"); // 타이틀 배경음악 재생
                    break;

                case SceneType.Ingame:
                    ChangeScene<IngameScene>(sceneType, sceneLoadAfterCallback);
                    // SoundManager.StopAll(); // 모든 사운드 정지
                    // SoundManager.PlayBGM("BGM_Ingame"); // 게임 배경음악 재생
                    break;

                default:
                    throw new System.NotImplementedException($"SceneType {sceneType} is not implemented."); 
            }
        }

        public void ReloadScene(SceneType sceneType = SceneType.None) // 현재 씬을 다시 로드하는 메서드
        {
            if (sceneType == SceneType.None) // 씬 타입이 지정되지 않은 경우 현재 씬 타입 사용
                sceneType = currentSceneType;

            switch (sceneType)
            {
                case SceneType.Title: // 타이틀 씬
                    ChangeScene<TitleScene>(sceneType);
                    break;

                case SceneType.Ingame: // 게임 씬
                    ChangeScene<IngameScene>(sceneType);
                    break;

                default:
                    throw new System.NotImplementedException($"SceneType {sceneType} is not implemented.");
            }
        }

        private void ChangeScene<T>(SceneType sceneType, System.Action sceneLoadAfterCallback = null) where T : SceneBase // 제네릭 씬 변경 메서드
        {
            StartCoroutine(ChangeSceneAsync<T>(sceneType, sceneLoadAfterCallback)); // 비동기 씬 변경 코루틴 시작
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, System.Action sceneLoadAfterCallback = null) where T : SceneBase // 비동기 씬 변경 코루틴
        {
            // UIManager.Singleton.HideAll(); // 모든 UI 숨김
            // var loadingUI = UIManager.Show<LoadingUI>(UIList.LoadingUI); // 로딩 UI 표시
            // loadingUI.SetLoadingProgress(0f); // 로딩 진행률 초기화

            yield return null; // 다음 프레임까지 대기

            if (sceneInstance) // 현재 씬 인스턴스가 존재하는 경우
            {
                yield return StartCoroutine(sceneInstance.OnEnd()); // 현재 씬 종료 코루틴 실행
                Destroy(sceneInstance.gameObject); // 현재 씬 인스턴스 게임 오브젝트 파괴
                sceneInstance = null; // 씬 인스턴스 변수 초기화
            }

            //loadingUI.SetLoadingProgress(0.25f); // 로딩 진행률 업데이트
            yield return null; // 다음 프레임까지 대기

            var async = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single); // 빈 씬 로드 시작
            while (!async.isDone) // 씬 로드가 완료될 때까지 대기
            {
                yield return null; // 다음 프레임까지 대기
            }

            // loadingUI.SetLoadingProgress(0.5f); // 로딩 진행률 업데이트
            yield return null;

            GameObject sceneInstanceGO = new GameObject(typeof(T).Name); // 새로운 씬 인스턴스 오브젝트 생성
            sceneInstanceGO.transform.SetParent(transform); // 현재 Main 오브젝트의 자식으로 설정
            sceneInstance = sceneInstanceGO.AddComponent<T>(); // 씬 인스턴스 컴포넌트 추가
            currentSceneType = sceneType; // 현재 씬 타입 업데이트

            yield return StartCoroutine(sceneInstance.OnStart()); // 씬 시작 처리
            // loadingUI.SetLoadingProgress(1f); // 로딩 진행률 업데이트
            yield return null; // 다음 프레임까지 대기

            // UIManager.Hide<LoadingUI>(UIList.LoadingUI); // 로딩 UI 숨김
            sceneLoadAfterCallback?.Invoke(); // 씬 로드 후 콜백 호출
        }
        public void SystemQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 모드 종료
#else
            Application.Quit(); // 빌드된 애플리케이션 종료
#endif
        }
    }
}

