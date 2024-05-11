using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour, Imanager {
    [SerializeField] private CustomButton _joinButton;
    [SerializeField] private CustomButton _createButton;
    [SerializeField] private CustomButton _startButton;
    [SerializeField] private CustomButton _copyRoomIDButton;
    [SerializeField] private CustomButton _backButton;
    [SerializeField] private TextMeshProUGUI _tmproInputFieldLabel;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI _tmproRoomID;

    private string _userID;


    public void Setup(params object[] parameters) {
        _joinButton.gameObject.SetActive(true);
        _createButton.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(false);

        _inputField.gameObject.SetActive(true);
        _inputField.text = string.Empty;
        _tmproInputFieldLabel.text = "User name";
        _tmproRoomID.gameObject.SetActive(false);

        _createButton.Setup(CreateRoom, GameManager.instance._colorPalette[1]);
        _joinButton.Setup(JoinRoom, GameManager.instance._colorPalette[5]);
        _startButton.Setup(StartGame, GameManager.instance._colorPalette[1]);
        _copyRoomIDButton.Setup(CopyRoomID, GameManager.instance._colorPalette[2]);
        _backButton.Setup(Back, GameManager.instance._colorPalette[2]);

        _userID = string.Empty;
    }

    private async Task ConnectionSetup() {
        bool goOn = false;

        GameManager.instance.loading.Load(true);
        _userID = string.IsNullOrEmpty(_inputField.text) ? $"Jean-Mich {Random.Range(0, 100)}" : _inputField.text;
        GameManager.instance.connectionManager.Setup(_userID, () => { goOn = true; });
        _inputField.text = string.Empty;

        while (!goOn)
            await Task.Yield();

        await Task.WhenAll(CenterButton(_createButton.gameObject, false), CenterButton(_joinButton.gameObject, true));
        GameManager.instance.loading.Load(false);
    }

    private async void CreateRoom() {
        await ConnectionSetup();

        _joinButton.gameObject.SetActive(false);
        _backButton.gameObject.SetActive(true);
        _inputField.gameObject.SetActive(false);
        _tmproRoomID.gameObject.SetActive(true);

        GameManager.instance.loading.Load(true);
        GameManager.instance.connectionManager.CreateRoom();
    }

    private async void JoinRoom() {
        if (string.IsNullOrEmpty(_userID)) {
            await ConnectionSetup();

            _createButton.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(true);
            _tmproInputFieldLabel.text = "Room id";

            if (GameManager.instance.debugMode)
                GameManager.instance.connectionManager.JoinRoom(AppConst.defaultRoomID);
            return;
        }

        if (!GameManager.instance.debugMode && string.IsNullOrEmpty(_inputField.text)) {
            Utils.Log(this, "JoinLobby", "id is empty");
            return;
        }

        GameManager.instance.loading.Load(true);
        GameManager.instance.connectionManager.JoinRoom(_inputField.text);
    }

    private void CopyRoomID() {
        GUIUtility.systemCopyBuffer = _tmproRoomID.text;
        GameManager.instance.messagePanel.ShowTemporary("ID copied in clipboard");
    }

    private void Back() {
        GameManager.instance.connectionManager.LeaveRoom();
        Setup();
        UncenterButton(_joinButton.gameObject, Vector2.zero, new Vector2(0.25f, 0.2f), true);
        UncenterButton(_createButton.gameObject, new Vector2(0.75f, 0), new Vector2(1, 0.2f), false);
    }

    public void OnJoinRoomSuccess(string roomID, string playerList) {
        GameManager.instance.loading.Load(false);
        _inputField.gameObject.SetActive(false);
        _tmproRoomID.gameObject.SetActive(true);
        _joinButton.gameObject.SetActive(false);
        _createButton.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(true);
        _tmproRoomID.text = roomID;

        string[] players = playerList.Split(';');
        foreach (string player in players)
            GameManager.instance.playerList.AddPlayer(int.Parse(player.Split('=')[0]), player.Split('=')[1]);
    }

    private void StartGame() {
        GameManager.instance.loading.Load(true);
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageStartGame);
    }


    private async Task CenterButton(GameObject button, bool startingFromLeft) {
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector2 min = new Vector2(0.375f, 0);
        Vector2 max = new Vector2(0.625f, 0.2f);

        if (startingFromLeft)
            rect.DOAnchorMax(max, AppConst.animDuration).SetEase(AppConst.animEase);
        else
            rect.DOAnchorMin(min, AppConst.animDuration).SetEase(AppConst.animEase);

        await GameManager.instance.TaskWithDelay(AppConst.animDuration);

        if (startingFromLeft)
            await rect.DOAnchorMin(min, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        else
            await rect.DOAnchorMax(max, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
    }

    private async void UncenterButton(GameObject button, Vector2 min, Vector2 max, bool goingLeft) {
        RectTransform rect = button.GetComponent<RectTransform>();

        if (goingLeft)
            rect.DOAnchorMin(min, AppConst.animDuration).SetEase(AppConst.animEase);
        else
            rect.DOAnchorMax(max, AppConst.animDuration).SetEase(AppConst.animEase);

        await GameManager.instance.TaskWithDelay(AppConst.animDuration);

        if (goingLeft)
            await rect.DOAnchorMax(max, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        else
            await rect.DOAnchorMin(min, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
    }
}
