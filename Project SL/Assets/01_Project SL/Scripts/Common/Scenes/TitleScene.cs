using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public class TitleScene : SceneBase
    {
        public override bool IsAdditiveScene => false; // 타이틀 씬은 단일 씬으로 로드됨

        public override IEnumerator OnStart()
        {
            var asyncSceneLoad = SceneManager.LoadSceneAsync(SceneType.Cathedral.ToString(), this.LoadSceneMode);
            while (!asyncSceneLoad.isDone)
            {
                yield return null; // 씬 로딩이 완료될 때까지 대기
            }

            // Cathedral 씬이 로드된 후, 한 프레임 대기 => Scene 안에 있는 System의 Awake()가 실행될 수 있도록
            yield return new WaitForEndOfFrame();

            CameraSystem.Instance.SetActiveTitleCamera(true); // 타이틀 카메라 활성화
            UIManager.Show<TitleUI>(UIList.TitleUI); // 씬 로딩이 완료되면 타이틀 UI 표시
        }

        public override IEnumerator OnEnd()
        {
            UIManager.Hide<TitleUI>(UIList.TitleUI); // 타이틀 UI 숨김
                                                     // 타이틀 씬 종료 시 필요한 작업이 있다면 여기에 추가

            yield return null; // 현재는 특별한 작업이 없으므로 바로 반환
        }
    }
}
