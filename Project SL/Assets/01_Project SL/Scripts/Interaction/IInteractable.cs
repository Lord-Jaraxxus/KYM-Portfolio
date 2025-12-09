using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum InteractableType
{
    DropItem,
    Portal,
    NPC,
    Lever,
    Whatever
}

namespace KYM
{
    public interface IInteractable
    {
        [SerializeField] InteractableType Type { get; }

        void Interact();    // 상호작용 메서드
        Transform GetTransform();   // Transform 반환 메서드
    }
}
