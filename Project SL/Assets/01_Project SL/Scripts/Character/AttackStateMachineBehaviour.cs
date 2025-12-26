using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class AttackStateMachineBehaviour : StateMachineBehaviour
    {
        private CharacterBase linkedCharacter;

        public void setCharacter(CharacterBase character)
        {
            this.linkedCharacter = character;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this.linkedCharacter.CurrentState = CharacterState.Attack;
        }


        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (linkedCharacter.CurrentState == CharacterState.Attack)   // 애니메이션에서 나갈 때 현재 상태가 Attack이라면 (중간에 방해받지 않았다면 (피격 등으로))
            {
                this.linkedCharacter.CurrentState = CharacterState.Idle;
                Debug.Log("Attack ended naturally → Idle");
            }
            else
            {
                Debug.Log("Attack interrupted → state preserved");
            }
        }
    }
}
