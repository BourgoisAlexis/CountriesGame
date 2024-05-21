using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace CountriesGame {
    public class Player : BasePlayer {
        public List<string> cardIDS = new List<string>();
    }

    public class ContestResult {
        public bool isValid { get; private set; }
        public string value { get; private set; }

        public ContestResult(bool isValid, string value) {
            this.isValid = isValid;
            this.value = value;
        }
    }

    public enum RoomStates {
        Lobby,

        GameInProgress,

        EmptyHand,
        KeepCurrentPlayer,
        GoForNextPlayer,

        GameEnded
    }

    [RoomType("Normal Game")]
    public class GameCode : Game<Player> {
        #region Variables
        private int _currentQueryIndex = 0;
        private int _initQueryIndex = -1;
        private int _themeQueryIndex = -1;
        private Dictionary<int, string> _contestQueriesResp;

        private Dictionary<string, string> _countryIDs;

        private List<string> _exceptionIDs = new List<string>() {
            "Q9419352"
        };

        private int _clearedBoards;

        private int _initDraw = 8;
        private string _currentTheme;
        private RoomStates _roomState;
        private int _currentPlayerIndex;
        private List<string> _availableCards;
        private List<string> _cardsOnBoard;

        private AppConst _appConst = new AppConst();
        private Random _random = new Random();
        #endregion


        #region Player.IO Methods
        // This method is called when an instance of your the game is created
        public override void GameStarted() {
            _roomState = RoomStates.Lobby;
            Console.WriteLine($"Room start : {RoomId}");
        }

        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed() {
            Console.WriteLine($"Room close : {RoomId}");
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player) {
            StringBuilder b = new StringBuilder();
            foreach (Player p in Players) {
                b.Append(p.Id);
                b.Append('=');
                b.Append(p.ConnectUserId);
                if (p != Players.Last())
                    b.Append(';');

                if (p != player)
                    p.Send(_appConst.serverMessageUserJoin, player.Id, player.ConnectUserId);
            }

            player.Send(_appConst.serverMessageJoinRoom, RoomId, player.Id, b.ToString());

            Console.WriteLine($"version {player.JoinData[_appConst.gameVersionKey]}");

            if (player.JoinData[_appConst.gameVersionKey] != _appConst.gameVersion)
                player.Send(_appConst.serverMessageError, "A new version is available, click <b>here</b> <link>https://mikadaux.itch.io/countriesgame</link> to download it.\nYou might encounter errors with your current version.");
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player) {
            Broadcast(_appConst.serverMessageLeaveRoom, player.Id);
        }
        #endregion


        private void Start() {
            _currentQueryIndex = 0;
            _contestQueriesResp = new Dictionary<int, string>();
            _countryIDs = new Dictionary<string, string>();
            _availableCards = new List<string>();
            _cardsOnBoard = new List<string>();
            _roomState = RoomStates.GameInProgress;

            foreach (Player p in Players)
                p.cardIDS = new List<string>();
        }

        public override bool AllowUserJoin(Player player) {
            return _roomState == RoomStates.Lobby;
        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message m) {
            LogMessage(m);

            switch (m.Type) {
                case "playermessage_startgame":
                    Broadcast(_appConst.serverMessageStartGame);
                    Start();
                    _initQueryIndex = _currentQueryIndex;
                    SendQuery("Init");
                    break;

                case "playermessage_playcard":
                    int onboardIndex = m.GetInt(0);
                    string cardID = m.GetString(1);
                    player.cardIDS.Remove(cardID);
                    _cardsOnBoard.Insert(onboardIndex, cardID);

                    foreach (Player p in Players)
                        if (p != null && p != player)
                            p.Send(_appConst.serverMessagePlayCard, onboardIndex, cardID, _countryIDs[cardID]);

                    if (player.cardIDS.Count <= 0) {
                        _roomState = RoomStates.EmptyHand;
                        OnContest();
                    }
                    else
                        NextPlayer();
                    break;

                case "playermessage_contest":
                    if (_cardsOnBoard.Count < 1) {
                        player.Send(_appConst.serverMessageError, "You can't contest while there is no card on board");
                        break;
                    }
                    OnContest();
                    break;

                case "playermessage_queryresult":
                    int queryIndex = m.GetInt(0);
                    string content = m.GetString(1);
                    HandleQueryResult(queryIndex, content);
                    break;

                case "playermessage_nextround":
                    _clearedBoards = 0;
                    _availableCards.AddRange(_cardsOnBoard);
                    _cardsOnBoard.Clear();
                    Broadcast(_appConst.serverMessageNextRound);
                    _currentTheme = SelectTheme();
                    _themeQueryIndex = _currentQueryIndex;
                    SendQuery("GetPropertyLabel", _currentTheme);
                    break;

                case "playermessage_clearboard":
                    _clearedBoards++;
                    if (_clearedBoards == Players.Count()) {
                        NextPlayer();
                        _roomState = RoomStates.GameInProgress;
                    }
                    break;

                case "playermessage_returntolobby":
                    Broadcast(_appConst.serverMessageReturnToLobby);
                    _roomState = RoomStates.Lobby;
                    break;
            }
        }


        #region Custom Methods
        private void SendQuery(string queryID, string propertyID = null, string countryID = null) {
            byte[] bytes = EmbeddedResource.GetBytes($"{queryID}.txt");
            string command = Encoding.Default.GetString(bytes);

            if (propertyID != null)
                command = command.Replace("propertyID", propertyID);

            if (countryID != null)
                command = command.Replace("countryID", countryID);

            Players.First().Send(_appConst.serverMessageProcessQuery, _currentQueryIndex, command);
            _currentQueryIndex++;
        }

        private void HandleQueryResult(int index, string content) {
            if (index == _initQueryIndex) {
                InitGame(content);
                return;
            }
            if (index == _themeQueryIndex) {
                string[] parts = content.Split(';');
                Broadcast(_appConst.serverMessageSelectTheme, parts[0].Replace("@en", ""), parts[1].Replace("@en", ""));

                int n = _random.Next(0, _availableCards.Count);
                string id = _availableCards[n];
                _availableCards.RemoveAt(n);
                _cardsOnBoard.Add(id);

                Broadcast(_appConst.serverMessagePlayCard, 0, id, _countryIDs[id]);

                return;
            }
            if (_contestQueriesResp.ContainsKey(index)) {
                _contestQueriesResp[index] = content;
                OnContestResponse();
            }
        }

        private void InitGame(string content) {
            InitCountryDictionnary(content);

            foreach (Player p in Players)
                for (int i = 0; i < _initDraw; i++)
                    DrawCard(p);

            _currentTheme = SelectTheme();
            _currentPlayerIndex = _random.Next(0, PlayerCount);
            _themeQueryIndex = _currentQueryIndex;
            SendQuery("GetPropertyLabel", _currentTheme);
            NextPlayer();
        }

        private void InitCountryDictionnary(string content) {
            string[] lines = content.Split(';');
            foreach (string line in lines) {

                string[] parts = line.Split('=');

                if (parts.Length < 2) {
                    Console.Error.WriteLine(line);
                    continue;
                }

                string cardID = parts[0];

                if (_exceptionIDs.Contains(cardID)) {
                    Console.Error.WriteLine($"{cardID} is an exception id");
                    continue;
                }

                _countryIDs.Add(cardID, parts[1]);
                _availableCards.Add(cardID);
            }
        }

        private string SelectTheme() {
            int result = _random.Next(0, _appConst.themes.Count);
            return _appConst.themes[result].id;
        }

        private void BroadcastCurrentPlayer() {
            StringBuilder b = new StringBuilder();
            foreach (Player p in Players) {
                b.Append(p.Id);
                b.Append('=');
                b.Append(p.cardIDS.Count);
                if (p != Players.Last())
                    b.Append(';');
            }

            Broadcast(_appConst.serverMessageCurrentPlayer, GetPlayerAtIndex(_currentPlayerIndex).Id, GetPreviousPlayer().Id, b.ToString());
        }

        private void NextPlayer() {
            if (_roomState != RoomStates.KeepCurrentPlayer) {
                _currentPlayerIndex++;
                if (_currentPlayerIndex > Players.Count() - 1)
                    _currentPlayerIndex = 0;
            }

            BroadcastCurrentPlayer();
        }

        private void DrawCard(Player player, int amount = 1) {
            for (int i = 0; i < amount; i++) {
                int n = _random.Next(0, _availableCards.Count);
                string id = _availableCards[n];
                player.Send(_appConst.serverMessageDrawCard, id, _countryIDs[id]);
                player.cardIDS.Add(id);
                _availableCards.RemoveAt(n);
            }
        }

        private void OnContest() {
            _contestQueriesResp.Clear();

            foreach (string cardID in _cardsOnBoard) {
                _contestQueriesResp.Add(_currentQueryIndex, null);

                string queryID = string.Empty;
                PropertyQuery query = _appConst.themes.Find(x => x.id == _currentTheme);

                switch (query.type) {
                    case PropertyType.MostRecent:
                        queryID = "GetMostRecentValue";
                        break;
                    case PropertyType.NestedProp:
                        queryID = "GetNestedPropertyValue";
                        break;
                    case PropertyType.Prop:
                        queryID = "GetPropertyValue";
                        break;
                }

                SendQuery(queryID, string.IsNullOrEmpty(query.idForQuery) ? query.id : query.idForQuery, cardID);
            }
        }

        private void OnContestResponse() {
            //Faire le tris entre les decimal et les datetime
            PropertyQuery query = _appConst.themes.Find(x => x.id == _currentTheme);

            bool goOn = true;
            foreach (KeyValuePair<int, string> resp in _contestQueriesResp)
                if (resp.Value == null) {
                    goOn = false;
                    break;
                }

            if (!goOn)
                return;

            List<ContestResult> results = new List<ContestResult>();
            var list = _contestQueriesResp.ToList();
            foreach (KeyValuePair<int, string> resp in _contestQueriesResp) {
                if (decimal.TryParse(resp.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v)) {
                    bool r = true;
                    foreach (KeyValuePair<int, string> pair in list.FindAll(x => x.Key < resp.Key)) {
                        if (decimal.TryParse(pair.Value.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal vv))
                            r = v > vv;
                        else
                            continue;

                        if (!r)
                            break;
                    }

                    var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = " ";
                    results.Add(new ContestResult(r, $"{v.ToString("#,0.00", nfi)} {query.unit}"));
                    continue;
                }

                results.Add(new ContestResult(true, "no data"));
            }

            StringBuilder b = new StringBuilder();

            for (int i = 0; i < results.Count; i++) {
                ContestResult r = results[i];
                b.Append(r.isValid);
                b.Append("=");
                b.Append(r.value);
                if (i < results.Count - 1)
                    b.Append(";");
            }

            Broadcast(_appConst.serverMessageContestResult, b.ToString(), GetPlayerAtIndex(_currentPlayerIndex).Id);

            if (_roomState == RoomStates.Lobby || _roomState == RoomStates.GameEnded)
                return;

            bool validContest = results.Find(x => x.isValid == false) != null;

            Console.WriteLine(_roomState);

            if (_roomState == RoomStates.EmptyHand) {
                if (validContest) {
                    _roomState = RoomStates.GoForNextPlayer;
                    DrawCard(GetPlayerAtIndex(_currentPlayerIndex), 2);
                }
                else {
                    _roomState = RoomStates.GameEnded;
                    Broadcast(_appConst.serverMessageGameEnded, GetPlayerAtIndex(_currentPlayerIndex).Id);
                }
            }
            else {
                if (validContest) {
                    _roomState = RoomStates.KeepCurrentPlayer;
                    DrawCard(GetPreviousPlayer(), 2);
                }
                else {
                    _roomState = RoomStates.GoForNextPlayer;
                    DrawCard(GetPlayerAtIndex(_currentPlayerIndex), 2);
                }
            }

            Console.WriteLine(_roomState);
        }

        private Player GetNextPlayer() {
            if (_currentPlayerIndex > Players.Count() - 2)
                return GetPlayerAtIndex(0);

            return GetPlayerAtIndex(_currentPlayerIndex + 1);
        }

        private Player GetPreviousPlayer() {
            if (_currentPlayerIndex < 1)
                return GetPlayerAtIndex(Players.Count() - 1);

            return GetPlayerAtIndex(_currentPlayerIndex - 1);
        }

        private Player GetPlayerAtIndex(int index) {
            int i = 0;
            foreach (Player p in Players) {
                if (i == index)
                    return p;

                i++;
            }

            return null;
        }

        private void LogMessage(Message m) {
            StringBuilder b = new StringBuilder();
            b.Append($"{m.Type} > ");
            for (int i = 0; i < m.Count; i++) {
                b.Append(m[(uint)i]);
                if (i < m.Count - 1)
                    b.Append(" ; ");
            }

            Console.WriteLine(b.ToString());
        }
        #endregion
    }
}