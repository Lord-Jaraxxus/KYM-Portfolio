namespace KYM
{
    // UI List의 이름은 UI Prefab 원본의 이름과 동일하다. (동일해야 한다)
    public enum UIList
    {
        POPUP_START, 

        CharacterInfoUI,
        CharacterEquipUI,
        MenuUI,
        DepthUI,
        BulletinBoardUI,
        DialogueUI,
        InventoryUI,
        ShopUI,

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
