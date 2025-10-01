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
        Title,  // Ÿ��Ʋ ��
        Ingame, // �ΰ��� ��
    }

    public class Main : SingletonBase<Main>
    {
        private bool isInitialized = false;
        private SceneType currentSceneType = SceneType.None;

        private SceneBase sceneInstance; // ���� �� �ν��Ͻ��� �����ϴ� ����

        private void Start()
        {
            Initialize();   // �ʱ�ȭ �޼��� ȣ��
        }

        private void Initialize()
        {
            if (isInitialized) return; // �̹� �ʱ�ȭ�� ��� �ߺ� ���� ����

            // var soundManagerPrefab = Resources.Load<GameObject>("Sound/Prefab/KYM.SoundManager"); // ���� �Ŵ��� ������ �ε�
            // var soundManagerInst = Instantiate(soundManagerPrefab); // ���� �Ŵ��� �ν��Ͻ� ����

            // UIManager.Singleton.Initialize(); // UIManager �ʱ�ȭ
            // GameDataModel.Singleton.Initialize(); // GameDataModel �ʱ�ȭ
            // UserDataModel.Singleton.Initialize(); // UserDataModel �ʱ�ȭ
#if UNITY_EDITOR
            UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            if (activeScene.name == "Main") 
            {
                ChangeScene(SceneType.Title); // Ÿ��Ʋ ������ ��ȯ
            }
#else
            ChangeScene(SceneType.Title); 
#endif
            isInitialized = true; // �ʱ�ȭ �Ϸ� �÷��� ����
        }

        public void ChangeScene(SceneType sceneType, System.Action sceneLoadAfterCallback = null)   // �� ���� �޼��� 
        {
            if (currentSceneType == sceneType) return; // ���� �� Ÿ�԰� �����Ϸ��� �� Ÿ���� ������ �ƹ� �۾��� ���� ����

            Time.timeScale = 1f; // �ð� ������ �ʱ�ȭ

            switch (sceneType)
            {
                case SceneType.Title:
                    ChangeScene<TitleScene>(sceneType, sceneLoadAfterCallback);
                    // SoundManager.StopAll(); // ��� ���� ����
                    // SoundManager.PlayBGM("BGM_Title"); // Ÿ��Ʋ ������� ���
                    break;

                case SceneType.Ingame:
                    ChangeScene<IngameScene>(sceneType, sceneLoadAfterCallback);
                    // SoundManager.StopAll(); // ��� ���� ����
                    // SoundManager.PlayBGM("BGM_Ingame"); // ���� ������� ���
                    break;

                default:
                    throw new System.NotImplementedException($"SceneType {sceneType} is not implemented."); 
            }
        }

        public void ReloadScene(SceneType sceneType = SceneType.None) // ���� ���� �ٽ� �ε��ϴ� �޼���
        {
            if (sceneType == SceneType.None) // �� Ÿ���� �������� ���� ��� ���� �� Ÿ�� ���
                sceneType = currentSceneType;

            switch (sceneType)
            {
                case SceneType.Title: // Ÿ��Ʋ ��
                    ChangeScene<TitleScene>(sceneType);
                    break;

                case SceneType.Ingame: // ���� ��
                    ChangeScene<IngameScene>(sceneType);
                    break;

                default:
                    throw new System.NotImplementedException($"SceneType {sceneType} is not implemented.");
            }
        }

        private void ChangeScene<T>(SceneType sceneType, System.Action sceneLoadAfterCallback = null) where T : SceneBase // ���׸� �� ���� �޼���
        {
            StartCoroutine(ChangeSceneAsync<T>(sceneType, sceneLoadAfterCallback)); // �񵿱� �� ���� �ڷ�ƾ ����
        }

        private IEnumerator ChangeSceneAsync<T>(SceneType sceneType, System.Action sceneLoadAfterCallback = null) where T : SceneBase // �񵿱� �� ���� �ڷ�ƾ
        {
            // UIManager.Singleton.HideAll(); // ��� UI ����
            // var loadingUI = UIManager.Show<LoadingUI>(UIList.LoadingUI); // �ε� UI ǥ��
            // loadingUI.SetLoadingProgress(0f); // �ε� ����� �ʱ�ȭ

            yield return null; // ���� �����ӱ��� ���

            if (sceneInstance) // ���� �� �ν��Ͻ��� �����ϴ� ���
            {
                yield return StartCoroutine(sceneInstance.OnEnd()); // ���� �� ���� �ڷ�ƾ ����
                Destroy(sceneInstance.gameObject); // ���� �� �ν��Ͻ� ���� ������Ʈ �ı�
                sceneInstance = null; // �� �ν��Ͻ� ���� �ʱ�ȭ
            }

            //loadingUI.SetLoadingProgress(0.25f); // �ε� ����� ������Ʈ
            yield return null; // ���� �����ӱ��� ���

            var async = SceneManager.LoadSceneAsync("Empty", LoadSceneMode.Single); // �� �� �ε� ����
            while (!async.isDone) // �� �ε尡 �Ϸ�� ������ ���
            {
                yield return null; // ���� �����ӱ��� ���
            }

            // loadingUI.SetLoadingProgress(0.5f); // �ε� ����� ������Ʈ
            yield return null;

            GameObject sceneInstanceGO = new GameObject(typeof(T).Name); // ���ο� �� �ν��Ͻ� ������Ʈ ����
            sceneInstanceGO.transform.SetParent(transform); // ���� Main ������Ʈ�� �ڽ����� ����
            sceneInstance = sceneInstanceGO.AddComponent<T>(); // �� �ν��Ͻ� ������Ʈ �߰�
            currentSceneType = sceneType; // ���� �� Ÿ�� ������Ʈ

            yield return StartCoroutine(sceneInstance.OnStart()); // �� ���� ó��
            // loadingUI.SetLoadingProgress(1f); // �ε� ����� ������Ʈ
            yield return null; // ���� �����ӱ��� ���

            // UIManager.Hide<LoadingUI>(UIList.LoadingUI); // �ε� UI ����
            sceneLoadAfterCallback?.Invoke(); // �� �ε� �� �ݹ� ȣ��
        }
        public void SystemQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� �÷��� ��� ����
#else
            Application.Quit(); // ����� ���ø����̼� ����
#endif
        }
    }
}

