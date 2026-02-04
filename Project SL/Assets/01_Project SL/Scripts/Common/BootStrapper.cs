#if UNITY_EDITOR

using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace KYM
{
    // 에디터 전용 부트스트랩퍼 클래스 (런타임 시 초기화 용도, 에디터에서만 동작)
    public class BootStrapper
    {
        // 에디터 메뉴에 표시될 경로 및 이름
        private const string BootStrapperMeunuPath = "PROJECT KYM/BootStrapper/Active BootStrapper";

        // 부트스트랩퍼가 활성 상태인지 여부를 저장/조회하는 프로퍼티
        private static bool IsActiveBootStrapper
        {
            get
            {
                // 에디터 환경 설정에서 부트스트랩퍼 활성 여부를 불러움
                bool isActive = UnityEditor.EditorPrefs.GetBool(BootStrapperMeunuPath, false);
                // 메뉴에 체크 표시 갱신
                UnityEditor.Menu.SetChecked(BootStrapperMeunuPath, isActive);
                return isActive;
            }
            set
            {
                // 에디터 환경 설정에 부트스트랩퍼 활성 여부를 저장
                UnityEditor.EditorPrefs.SetBool(BootStrapperMeunuPath, value);
                UnityEditor.Menu.SetChecked(BootStrapperMeunuPath, value);
            }
        }

        // 에디터 메뉴에서 부트스트랩퍼 활성/비활성 토글
        [UnityEditor.MenuItem(BootStrapperMeunuPath, false)]
        private static void ActiveBootStrapper()
        {
            IsActiveBootStrapper = !IsActiveBootStrapper;
        }

        // 런타임 초기화 : 씬 로드 전에 실행됨 (단, 에디터에서 실행 시에만 작동)
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void SystemBoot()
        {
            // 현재 에디터에서 활성화된 씬을 가져옴
            UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            // 부트스트랩퍼가 활성화되어 있고, 현재 씬이 "Main" 씬이 아닐 경우 초기화 실행
            if (IsActiveBootStrapper && false == activeScene.name.Equals("Main"))
            {
                InternalBoot();
            }
        }

        //실제 초기화 로직을 담당하는 메서드
        private static void InternalBoot()
        {
            // Main 싱글톤 초기화 (예: 게임 전체 컨텍스트 초기화)
            Main.Singleton.Initialize();

            // Main.Singleton.ReloadScene(SceneType.Ingame); // 이렇게 하면 자동으로 IngameScene 스크립트의 OnStart 부를 수 있지 않나? 굳이 밑에서 UI 일일히 안켜줘도 

            // 필요한 초기 UI 호출 (예 : HUD 표시)
             UIManager.Show<PlayerHUD>(UIList.PlayerHUD);
             UIManager.Show<GlobalUI>(UIList.GlobalUI);

            // 하고싶은 커스텀 로직을 추가하세요.
        }
    }
}

#endif