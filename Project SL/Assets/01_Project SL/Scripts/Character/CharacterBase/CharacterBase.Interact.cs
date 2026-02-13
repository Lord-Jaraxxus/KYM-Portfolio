using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KYM
{
    public partial class CharacterBase
    {
        public bool CanInteract()
        {
            return !interactBlockedState.Contains(CurrentState); // 상호작용 가능한 상태인지 반환
        }

        public void TryInteract(InteractableType type)
        {
            if (interactBlockedState.Contains(CurrentState)) { return; } // 상호작용 불가 상태일 경우 Interact 함수 종료

            // TODO : 이 밑에서 이제 상호작용 종류를 switch문 같은걸로 나눠서 
            switch (type)
            {
                case InteractableType.DropItem: // 드롭 아이템과 상호작용 했을 때
                    CurrentState = CharacterState.Interact; // 상호작용 상태로 전환, 줍기 애니메이션 전에 해줘야 여러번 줍는 버그가 안남..
                    Root(); // 상호작용 애니메이션 재생, 애니메이션이 끝나면 Idle 상태로 돌아감
                    break;
                case InteractableType.NPC_Merchant: // 상인 NPC와 상호작용 했을 때
                    // 뭐 별로 할거 없긴 함..
                    break;
                default:
                    break;
            }
        }
        public void Root()
        {
            animator.SetTrigger("RootTrigger");
        }
    }
}
