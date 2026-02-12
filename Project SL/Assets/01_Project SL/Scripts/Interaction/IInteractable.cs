using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractableType
{
    DropItem,
    Portal,
    Lever,

    NPC_Merchant,
    NPC_Dialogue,
    NPC_QuestGiver,
    NPC_Entrance,
}

namespace KYM
{
    public interface IInteractable
    {
        InteractableType type { get; }

        void Interact();    // 상호작용 메서드
        Transform GetTransform();   // Transform 반환 메서드
    }
}
