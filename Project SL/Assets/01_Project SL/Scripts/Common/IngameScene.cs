using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KYM
{
    public class IngameScene : SceneBase
    {
        public override bool IsAdditiveScene => false; // Ingame ���� ���� ������ �ε��

        public override IEnumerator OnStart()
        {
            var asyncSceneLoad = SceneManager.LoadSceneAsync(SceneType.Ingame.ToString(), this.LoadSceneMode);
            while (!asyncSceneLoad.isDone)
            {
                yield return null; // �� �ε尡 �Ϸ�� ������ ���
            }

            // UIManager.Show<PlayerHUD>(UIList.PlayerHUD); // Ÿ��Ʋ UI ǥ��
        }

        public override IEnumerator OnEnd()
        {
            // UIManager.Hide<PlayerHUD>(UIList.PlayerHUD); // Ÿ��Ʋ UI ����
                                                         // Ÿ��Ʋ �� ���� �� �ʿ��� �۾��� �ִٸ� ���⿡ �߰�

            yield return null; // ����� Ư���� �۾��� �����Ƿ� �ٷ� ��ȯ
        }
    }
}
