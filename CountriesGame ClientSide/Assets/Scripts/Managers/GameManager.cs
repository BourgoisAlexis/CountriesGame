using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    #region Variables
    public static GameManager instance;

    private bool _setuped;

    public List<Color> _colorPalette = new List<Color>();
    public ConnectionManager connectionManager;
    public SPARQLManager sparqlManager;
    public InputManager inputManager;
    public Loading loading { get; private set; }
    public ViewManager viewManager { get; private set; }
    public LobbyManager lobbyManager { get; private set; }
    public BoardManager boardManager { get; private set; }

    public PlayerList playerList { get; private set; }
    public MessagePanel messagePanel { get; private set; }

    public GameObject prefabCard;
    public GameObject prefabCardController;
    public GameObject playerPrefab;

    [Header("Debug")]
    public bool debugMode = true;
    public bool gameEnded;
    #endregion


    private void Awake() {
        if (instance == null)
            instance = this;

        connectionManager = new ConnectionManager();
        sparqlManager = new SPARQLManager();
        inputManager = new InputManager();
        loading = FindObjectOfType<Loading>(true);
        viewManager = FindObjectOfType<ViewManager>();
        lobbyManager = FindObjectOfType<LobbyManager>(true);
        boardManager = FindObjectOfType<BoardManager>(true);

        playerList = FindObjectOfType<PlayerList>(true);
        messagePanel = FindObjectOfType<MessagePanel>(true);
    }

    private void Start() {
        inputManager.Setup(FindObjectsOfType<GraphicRaycaster>(), FindObjectOfType<EventSystem>());
        inputManager.Enable(InteractableTags.LobbyButton);
        viewManager.Setup(0);
        lobbyManager.Setup();
        boardManager.Setup();
        messagePanel.Hide();

        _setuped = true;
    }

    private void Update() {
        if (!_setuped)
            return;

        //connectionManager.ProcessMessages();
        inputManager.Update();
    }

    public async void ReturnToLobby() {
        messagePanel.Hide();
        await boardManager.HandController.Clear();
        await boardManager.DropZone.Clear();

        viewManager.ShowView(0);
        inputManager.Enable(InteractableTags.LobbyButton);
        playerList.LobbyState();
    }


    public async Task TaskWithDelay(float duration) {
        bool goOn = false;
        StartCoroutine(WaitCoroutine(duration, () => { goOn = true; }));
        while (!goOn)
            await Task.Yield();
    }

    public System.Collections.IEnumerator WaitCoroutine(float duration, Action onEnd) {
        yield return new WaitForSecondsRealtime(duration);
        onEnd?.Invoke();
    }
}
