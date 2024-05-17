using DG.Tweening;

public enum InteractableTags {
    Default,

    CardController,
    LobbyButton,
    RulesButton,
    ActionButton,
    DropZone
}

public static class AppConst {
    public const int sortBase = 1;
    public const int sortHover = 99;
    public const int sortDrag = 100;

    public const float animDuration = 0.1f;
    public const Ease animEase = Ease.InOutSine;

    public const string defaultRoomID = "defaultRoom";

    //Messages Sent
    public const string playerMessagePlayCard = "playermessage_playcard";
    public const string playerMessageContest = "playermessage_contest";
    public const string playerMessageStartGame = "playermessage_startgame";
    public const string playerMessageQueryResult = "playermessage_queryresult";
    public const string playerMessageNextRound= "playermessage_nextround";
    public const string playerMessageClearBoard = "playermessage_clearboard";
    public const string playerMessageReturnToLobby = "playermessage_returntolobby";
    //Messages Received
    public const string serverMessageError = "servermessage_error";
    public const string serverMessageUserJoin = "servermessage_userjoin";
    public const string serverMessageJoinRoom = "servermessage_joinroom";
    public const string serverMessageLeaveRoom = "servermessage_leaveroom";
    public const string serverMessageStartGame = "servermessage_startgame";
    public const string serverMessageSelectTheme = "servermessage_selecttheme";
    public const string serverMessageCurrentPlayer = "servermessage_currentplayer";
    public const string serverMessageDrawCard = "servermessage_drawcard";
    public const string serverMessagePlayCard = "servermessage_playcard";
    public const string serverMessageProcessQuery = "servermessage_processquery";
    public const string serverMessageContestResult = "servermessage_contestresult";
    public const string serverMessageNextRound = "servermessage_nextround";
    public const string serverMessageGameEnded = "servermessage_gameended";
    public const string serverMessageReturnToLobby = "servermessage_returntolobby";
}
