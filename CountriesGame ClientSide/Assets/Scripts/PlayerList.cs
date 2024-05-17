using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour {
    private Dictionary<int, PlayerUI> _players = new Dictionary<int, PlayerUI>();

    public void AddPlayer(int playerID, string userName) {
        if (_players.ContainsKey(playerID))
            return;

        GameObject instantiatedObj = Instantiate(GameManager.instance.playerPrefab, transform);
        PlayerUI playerUI = instantiatedObj.GetComponent<PlayerUI>();

        playerUI.Setup(playerID, userName);
        _players.Add(playerID, playerUI);
    }

    public void RemovePlayer(int playerID) {
        if (!_players.ContainsKey(playerID))
            return;

        GameObject obj = _players[playerID].gameObject;
        _players.Remove(playerID);
        Destroy(obj);
    }

    public void Clear() {
        List<int> ids = new List<int>();
        foreach (KeyValuePair<int, PlayerUI> p in _players)
            ids.Add(p.Key);

        foreach (int i in ids)
            RemovePlayer(i);
    }

    public string GetPlayerName(int playerID) {
        return _players[playerID].Name;
    }

    public void HighlightPlayers(int currentPlayerID, int previousPlayerID) {
        foreach (KeyValuePair<int, PlayerUI> player in _players)
            player.Value.Highlight(currentPlayerID, previousPlayerID);
    }

    public void UpdateCardCounts(string counts) {
        string[] playerCounts = counts.Split(';');
        foreach (string s in playerCounts) {
            int id = int.Parse(s.Split('=')[0]);
            int count = int.Parse(s.Split('=')[1]);

            _players[id].UpdateCardCount(count);
        }
    }

    public void LobbyState() {
        foreach (KeyValuePair<int, PlayerUI> player in _players)
            player.Value.Lobby();
    }
}
