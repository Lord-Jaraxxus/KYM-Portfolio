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
                yield return null; // �� �ε��� �Ϸ�� ������ ���
            }

           //UIManager.Show<TitleUI>(UIList.TitleUI); // Ÿ��Ʋ UI ǥ��
        }

        public override IEnumerator OnEnd()
        {
            //UIManager.Hide<TitleUI>(UIList.TitleUI); // Ÿ��Ʋ UI ����
                                                     // Ÿ��Ʋ �� ���� �� �ʿ��� �۾��� �ִٸ� ���⿡ �߰�

            yield return null; // ����� Ư���� �۾��� �����Ƿ� �ٷ� ��ȯ
        }
    }
}
