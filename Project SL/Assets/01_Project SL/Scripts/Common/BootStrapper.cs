#if UNITY_EDITOR

using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace KYM
{
    public class BootStrapper
    {
        private const string BootStrapperMeunuPath = "PROJECT KYM/BootStrapper/Active BootStrapper";

        private static bool IsActiveBootStrapper
        {
            get
            {
                bool isActive = UnityEditor.EditorPrefs.GetBool(BootStrapperMeunuPath, false);
                UnityEditor.Menu.SetChecked(BootStrapperMeunuPath, isActive);
                return isActive;
            }
            set
            {
                UnityEditor.EditorPrefs.SetBool(BootStrapperMeunuPath, value);
                UnityEditor.Menu.SetChecked(BootStrapperMeunuPath, value);
            }
        }

        [UnityEditor.MenuItem(BootStrapperMeunuPath, false)]

        private static void ActiveBootStrapper()
        {
            IsActiveBootStrapper = !IsActiveBootStrapper;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        public static void SystemBoot()
        {
            UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (IsActiveBootStrapper && false == activeScene.name.Equals("Main"))
            {
                InternalBoot();
            }
        }

        private static void InternalBoot()
        {
            Main.Singleton.Initialize();
            // 하고싶은 커스텀 로직을 추가하세요.

            // UIManager.Show<PlayerHUD>(UIList.PlayerHUD);
            // UIManager.Show<CrosshairUI>(UIList.CrosshairUI);
        }
    }
}

#endif