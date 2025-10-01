using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public class IngameScene : SceneBase
    {
        public override bool IsAdditiveScene => false; // Ingame 씬은 단일 씬으로 로드됨

        public override IEnumerator OnStart()
        {
            var asyncSceneLoad = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), this.LoadSceneMode);
            while (!asyncSceneLoad.isDone)
            {
                yield return null; // 씬 로드가 완료될 때까지 대기
            }

            // UIManager.Show<PlayerHUD>(UIList.PlayerHUD); // 타이틀 UI 표시
        }

        public override IEnumerator OnEnd()
        {
            // UIManager.Hide<PlayerHUD>(UIList.PlayerHUD); // 타이틀 UI 숨김
                                                         // 타이틀 씬 종료 시 필요한 작업이 있다면 여기에 추가

            yield return null; // 현재는 특별한 작업이 없으므로 바로 반환
        }
    }
}
