using UnityEngine;
using System.Collections.Generic;
using PlayerIOClient;
using System;

public class ConnectionManager {
    private Connection _connection;
    private Client _client;
    private int _id;
    private bool _isMyTurn;

    public bool MyTurn => _isMyTurn;

    private List<Message> _messages = new List<Message>(); //  Messsage queue implementation

    public void Setup(string userId, Action onSuccess) {
        Utils.Log(this, "Setup");
        Application.runInBackground = true;

        PlayerIO.Authenticate(
            "countries-leygqey2lewhmpwnsn93gw",     //Game ID         
            "public",                               //Connection ID
            new Dictionary<string, string> {        //Auth arguments
				{ "userId", userId },
            },
            null,                                   //PlayerInsight segments
            (Client client) => {
                _client = client;
                AuthenticateSuccess();
                onSuccess?.Invoke();
            },
            (PlayerIOError error) => {
                Utils.Log(this, "Setup", $"Auth error {error}");
                GameManager.instance.messagePanel.ShowError($"An error has occured : {error.Message}");
            }
        );
    }

    private void AuthenticateSuccess() {
        Utils.Log(this, "AuthenticateSuccess");

        if (!CheckClient())
            return;

        if (GameManager.instance.debugMode) {
            Utils.Log(this, "AuthenticateSuccess", "Create serverEndpoint");
            _client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);
        }
    }

    public void CreateRoom() {
        Utils.Log(this, "CreateRoom");

        if (!CheckClient())
            return;

        _client.Multiplayer.CreateRoom(
            GameManager.instance.debugMode ? AppConst.defaultRoomID : null,                               //Room id. If set to null a random roomid is used
            "Normal Game",                      //The room type started on the server
            true,                               //Should the room be visible in the lobby?
            null,
            (string roomId) => {
                JoinRoom(roomId);
            },
            (PlayerIOError error) => {
                Utils.Log(this, "CreateRoom", $"CreateRoom error {error}");
                GameManager.instance.messagePanel.ShowError($"An error has occured : {error.Message}");
            }
        );
    }

    public void JoinRoom(string roomId) {
        Utils.Log(this, "JoinRoom");

        if (!CheckClient())
            return;

        _client.Multiplayer.JoinRoom(
            roomId,                             //Room id. If set to null a random roomid is used
            new Dictionary<string, string> { 
                { "gameVersion", $"{Application.version}" } 
            },
            (Connection connection) => {
                _connection = connection;
                _connection.OnMessage += ReceiveMessage;
            },
            (PlayerIOError error) => {
                Utils.Log(this, "JoinRoom", $"JoinRoom error {error}");
                GameManager.instance.messagePanel.ShowError($"An error has occured : {error.Message}");
                GameManager.instance.loading.Load(false);
            }
        );
    }

    public void LeaveRoom() {
        Utils.Log(this, "LeaveRoom");

        if (_connection == null)
            return;

        _connection.Disconnect();
        GameManager.instance.playerList.Clear();
    }

    //Messages
    private void ReceiveMessage(object sender, Message m) {
        Utils.Log(this, "ReceiveMessage", $"{m.Type}");
        _messages.Add(m);
    }

    public void SendMessage(string type, params object[] parameters) {
        if (!CheckConnection())
            return;

        Message m = Message.Create(type, parameters);
        _connection.Send(m);
    }

    public void ProcessMessages() {
        foreach (Message m in _messages) {
            switch (m.Type) {
                case AppConst.serverMessageError:
                    string error = m.GetString(0);
                    GameManager.instance.messagePanel.ShowError(error);
                    if (error.Contains("can't contest")) {
                        GameManager.instance.loading.Load(false);
                        GameManager.instance.inputManager.Enable();
                    }
                    break;

                case AppConst.serverMessageDrawCard:
                    DataCard d = new DataCard(m.GetString(0), m.GetString(1));
                    GameManager.instance.boardManager.HandController.DrawCard(d);
                    break;

                case AppConst.serverMessagePlayCard:
                    DataCard dd = new DataCard(m.GetString(1), m.GetString(2));
                    GameManager.instance.boardManager.DropZone.InsertCard(m.GetInt(0), dd);
                    break;

                case AppConst.serverMessageStartGame:
                    GameManager.instance.viewManager.ShowView(1);
                    GameManager.instance.inputManager.Disable();
                    GameManager.instance.gameEnded = false;
                    break;

                case AppConst.serverMessageJoinRoom:
                    _id = m.GetInt(1);
                    GameManager.instance.lobbyManager.OnJoinRoomSuccess(m.GetString(0), m.GetString(2));
                    break;

                case AppConst.serverMessageLeaveRoom:
                    GameManager.instance.playerList.RemovePlayer(m.GetInt(0));
                    break;

                case "groupdisallowedjoin":
                    GameManager.instance.loading.Load(false);
                    GameManager.instance.messagePanel.ShowError("A game is already in progress");
                    break;

                case AppConst.serverMessageUserJoin:
                    GameManager.instance.playerList.AddPlayer(m.GetInt(0), m.GetString(1));
                    break;

                case AppConst.serverMessageSelectTheme:
                    GameManager.instance.boardManager.SetTheme(m.GetString(0));
                    GameManager.instance.loading.Load(false);
                    break;

                case AppConst.serverMessageCurrentPlayer:
                    _isMyTurn = m.GetInt(0) == _id;

                    if (_isMyTurn)
                        GameManager.instance.inputManager.Enable();
                    else
                        GameManager.instance.inputManager.Disable();

                    GameManager.instance.playerList.HighlightCurrentPlayer(m.GetInt(0));
                    break;

                case AppConst.serverMessageProcessQuery:
                    GameManager.instance.sparqlManager.ProcessQuery(m.GetInt(0), m.GetString(1));
                    break;

                case AppConst.serverMessageContestResult:
                    Utils.Log(this, "ProcessQuery", m.GetString(0));
                    GameManager.instance.boardManager.ShowResult(m.GetString(0));
                    GameManager.instance.loading.Load(false);

                    if (m.GetInt(1) == _id)
                        GameManager.instance.inputManager.Disable(true);
                    break;

                case AppConst.serverMessageNextRound:
                    GameManager.instance.messagePanel.ShowTemporary("next round");
                    GameManager.instance.boardManager.ClearBoard();
                    break;

                case AppConst.serverMessageGameEnded:
                    bool IWon = m.GetInt(0) == _id;
                    GameManager.instance.messagePanel.Show(IWon ? "You win !" : $"{GameManager.instance.playerList.GetPlayerName(m.GetInt(0))} win !");
                    GameManager.instance.gameEnded = true;
                    break;

                case AppConst.serverMessageReturnToLobby:
                    GameManager.instance.ReturnToLobby();
                    break;
            }
        }

        _messages.Clear();
    }

    private bool CheckClient() {
        if (_client == null) {
            Debug.LogError("_client is null");
            return false;
        }

        return true;
    }

    private bool CheckConnection() {
        if (_connection == null) {
            Debug.LogError("_connection is null");
            return false;
        }

        return true;
    }
}
