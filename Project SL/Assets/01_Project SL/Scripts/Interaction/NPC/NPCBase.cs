using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYM
{
    public class NPCBase : MonoBehaviour, IInteractable
    {
        [field:SerializeField] public InteractableType type { get; private set; }
        [SerializeField] public InteractableType Type => type;

        // 얘들은 좀 쪼개고싶은데 일단 귀찮으니까..
        [SerializeField] private string shopID;
        [SerializeField] private SceneType targetScene;

        public Transform GetTransform()
        {
            return this.transform;
        }

        public void Interact()
        {
            switch (type)
            {
                case InteractableType.NPC_Merchant: // 상인 NPC일 경우
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
                case InteractableType.NPC_Entrance: // 말걸면 던전으로 보내주는 NPC일 경우
                    Main.Singleton.ChangeScene(targetScene);
                    break;
                default:
                    break;
            }
        }
    }
}
