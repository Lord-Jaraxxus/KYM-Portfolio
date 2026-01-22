using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class HitStateMachineBehaviour : StateMachineBehaviour
    {
        private CharacterBase linkedCharacter;

        public void setCharacter(CharacterBase character)
        {
            this.linkedCharacter = character;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this.linkedCharacter.SetCharacterState(CharacterState.Hit);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (linkedCharacter.CurrentState == CharacterState.Hit)   // 애니메이션에서 나갈 때 현재 상태가 Hit이라면 (연속 피격 등으로 중간에 방해받지 않았다면)
            {
                this.linkedCharacter.SetCharacterState(CharacterState.Idle);
                Debug.Log("Hit ended naturally → Idle");
            }
            else // 중간에 방해받았다면
            {
                Debug.Log("Hit interrupted → state preserved");
            }
        }
    }
}
