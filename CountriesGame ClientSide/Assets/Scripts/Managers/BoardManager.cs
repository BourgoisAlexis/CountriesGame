using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _tmproTheme;
    [SerializeField] private DropZone _dropZone;
    [SerializeField] private HandController _handController;
    [SerializeField] private CustomButton _actionButton;
    [SerializeField] private Transform _outOfBoundsPos;

    private Color _enabled => GameManager.instance._colorPalette[1];
    private Color _disabled => GameManager.instance._colorPalette[5];
    private Color _actionButtonColor => GameManager.instance.connectionManager.MyTurn ? _enabled : _disabled;

    public DropZone DropZone => _dropZone;
    public HandController HandController => _handController;
    public Vector2 OutOfBoundsPos => _outOfBoundsPos.position;


    public void SetTheme(string theme) {
        _tmproTheme.text = theme;
        _actionButton.Setup(Contest, _actionButtonColor, "Contest");
    }

    public void SetActionButton(bool interactable) {
        _actionButton.SetInteractable(interactable);
        _actionButton.SetColor(_actionButtonColor);
    }

    public void ShowResult(string result) {
        List<DataContestResult> results = new List<DataContestResult>();
        string[] res = result.Split(";");

        foreach (string s in res) {
            string[] ss = s.Split('=');
            results.Add(new DataContestResult(bool.Parse(ss[0].ToLower()), ss[1]));
        }

        _dropZone.Reveal(results);

        if (GameManager.instance.gameEnded) {
            Action onClick = () => GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageReturnToLobby);
            _actionButton.Setup(onClick, _actionButtonColor, "Return to lobby");
            return;
        }

        _actionButton.Setup(NextRound, _actionButtonColor, "Next round");
    }

    public void Contest() {
        GameManager.instance.loading.Load(true);
        GameManager.instance.inputManager.Disable();
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageContest);
    }

    public void NextRound() {
        GameManager.instance.loading.Load(true);
        GameManager.instance.inputManager.Disable();
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageNextRound);
    }

    public async void ClearBoard() {
        await _dropZone.Clear();
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageClearBoard);
    }
}
