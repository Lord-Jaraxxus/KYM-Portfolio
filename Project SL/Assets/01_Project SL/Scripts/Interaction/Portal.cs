using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class Portal : MonoBehaviour, IInteractable
    {
        [SerializeField] private SceneType targetScene;
        public InteractableType Type { get; } = InteractableType.Portal;

        public void Interact()
        {
            Main.Singleton.ChangeScene(targetScene);
        }

        public Transform GetTransform() => this.transform;
    }
}
