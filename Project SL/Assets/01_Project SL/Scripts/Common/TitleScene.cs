using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public class TitleScene : SceneBase
    {
        public override bool IsAdditiveScene => false;

        public override IEnumerator OnStart()
        {
            var asyncSceneLoad = SceneManager.LoadSceneAsync(SceneType.Title.ToString(), this.LoadSceneMode);
            while (!asyncSceneLoad.isDone)
            {
                yield return null; // 씬 로딩이 완료될 때까지 대기
            }

           //UIManager.Show<TitleUI>(UIList.TitleUI); // 타이틀 UI 표시
        }

        public override IEnumerator OnEnd()
        {
            //UIManager.Hide<TitleUI>(UIList.TitleUI); // 타이틀 UI 숨김
                                                     // 타이틀 씬 종료 시 필요한 작업이 있다면 여기에 추가

            yield return null; // 현재는 특별한 작업이 없으므로 바로 반환
        }
    }
}
