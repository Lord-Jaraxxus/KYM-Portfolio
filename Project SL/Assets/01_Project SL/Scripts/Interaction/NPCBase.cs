using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class NPCBase : MonoBehaviour, IInteractable
    {
        [SerializeField] public InteractableType Type => InteractableType.NPC_Merchant;
        [SerializeField] private string shopID;

        public Transform GetTransform()
        {
            return this.transform;
        }

        private void Start()
        {
        }

        public void Interact()
        {
            switch (Type)
            {
                case InteractableType.NPC_Merchant:
                    ShopUI shopUI = UIManager.Singleton.GetUI<ShopUI>(UIList.ShopUI); 
                    bool isOpen = shopUI != null && shopUI.gameObject.activeSelf;

                    if (!isOpen)
                    {
                        shopUI.shopID = shopID;
                        UIManager.Show<ShopUI>(UIList.ShopUI);
                    }
                    break;
                case InteractableType.NPC_Dialogue:
                    // UIManager.Singleton.OpenDialogueUI();
                    break;
                default:
                    break;
            }
        }
    }
}
