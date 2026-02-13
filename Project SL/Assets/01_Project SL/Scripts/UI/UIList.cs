namespace KYM
{
    // UI List의 이름은 UI Prefab 원본의 이름과 동일하다. (동일해야 한다)

    // - Poopup과 Panel을 구분하는 방법 : 
    // Popup : ESC키를 눌렀을 때, 자동으로 닫히는 유형
    // Panel : ESC키를 눌러도 닫히지 않는 유형 
    // 으로 구분해서 생각하면 편하다. 

    public enum UIList
    {
        POPUP_START, 

        CharacterInfoUI,
        CharacterEquipUI,
        CharacterSkillUI,
        MenuUI,
        DepthUI,
        BulletinBoardUI,
        DialogueUI,
        InventoryUI,
        ShopUI,
        ConfirmUI,

        POPUP_END, 
        PANEL_START,

        LoadingUI,
        TitleUI,
        PlayerHUD,
        CrosshairUI,
        GameOverUI,
        InteractionUI,
        GlobalUI,

        PANEL_END,
    }
}
