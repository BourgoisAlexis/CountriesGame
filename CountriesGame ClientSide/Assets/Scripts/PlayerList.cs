using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour {
    private Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    private Dictionary<int, string> _playerNames = new Dictionary<int, string>();

    public void AddPlayer(int playerID, string userName) {
        if (_players.ContainsKey(playerID))
            return;

        GameObject instantiatedObj = Instantiate(GameManager.instance.playerPrefab, transform);
        instantiatedObj.GetComponentInChildren<TextMeshProUGUI>().text = userName;
        _players.Add(playerID, instantiatedObj);
        _playerNames.Add(playerID, userName);
    }

    public void RemovePlayer(int playerID) {
        if (!_players.ContainsKey(playerID))
            return;

        GameObject obj = _players[playerID];
        _players.Remove(playerID);
        _playerNames.Remove(playerID);
        Destroy(obj);
    }

    public void Clear() {
        List<int> ids = new List<int>();
        foreach (KeyValuePair<int, GameObject> p in _players)
            ids.Add(p.Key);

        foreach (int i in ids)
            RemovePlayer(i);
    }

    public string GetPlayerName(int playerID) {
        return _playerNames[playerID];
    }

    public void HighlightCurrentPlayer(int playerID) {
        foreach (KeyValuePair<int, GameObject> player in _players)
            player.Value.GetComponentInChildren<Image>().color = player.Key == playerID ? GameManager.instance._colorPalette[1] : GameManager.instance._colorPalette[5];
    }

    public void LobbyState() {
        foreach (KeyValuePair<int, GameObject> player in _players)
            player.Value.GetComponentInChildren<Image>().color = GameManager.instance._colorPalette[2];
    }
}
