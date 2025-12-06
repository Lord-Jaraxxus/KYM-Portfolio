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
            this.linkedCharacter.CurrentState = CharacterState.Idle;
        }
    }
}
