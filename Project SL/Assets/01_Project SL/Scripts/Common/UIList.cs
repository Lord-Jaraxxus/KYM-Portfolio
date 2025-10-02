namespace KYM
{
    // UI List의 이름은 UI Prefab 원본의 이름과 동일하다. (동일해야 한다)
    public enum UIList
    {
        POPUP_START, // 게임 시작 팝업 UI

        CharacterInfoUI,
        MenuUI,
        DepthUI,
        BulletinBoardUI,
        DialogueUI,

        POPUP_END, // 게임 종료 팝업 UI
        PANEL_START,

        LoadingUI,
        TitleUI,
        PlayerHUD,
        CrosshairUI,
        GameOverUI,
        InteractionUI,

        PANEL_END,
    }
}
