using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KYM
{
    public class SoundManager : SingletonBase<SoundManager>
    {
        public const string BGMCategoryName = "BGM"; // BGM 카테고리 이름
        public const string SFXCategoryName = "SFX"; // SFX 카테고리 이름

        // Volume Range : 0 ~ 1
        public static float VolumeOfBGM
        {
            set => SetVolume(BGMCategoryName, value);
            get => AudioController.GetCategoryVolume(BGMCategoryName);
        }

        // Volume Range : 0 ~ 1
        public static float VolumeOfSFX
        {
            set => SetVolume(SFXCategoryName, value);
            get => AudioController.GetCategoryVolume(SFXCategoryName);
        }


        public static void SetVolume(string categoryName, float volume /* Range: 0 ~ 1 */)
        {
            AudioController.SetCategoryVolume(categoryName, volume);
        }

        public static void PlaySFX(string soundName, Vector3 position)
        {
            AudioController.Play(soundName, position);
        }

        public static void StopSFX(string soundName)
        {
            AudioController.Stop(soundName);
        }

        public static void PlayBGM(string bgmName)
        {
            AudioController.PlayMusic(bgmName);
        }

        public static void StopBGM(string bgmName)
        {
            AudioController.Stop(bgmName);
        }

        public static void StopAll()
        {
            AudioController.StopAll();
        }
    }
}
