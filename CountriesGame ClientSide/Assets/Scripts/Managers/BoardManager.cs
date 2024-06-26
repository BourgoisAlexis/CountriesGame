using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour, Imanager {
    [SerializeField] private TextMeshProUGUI _tmproTheme;
    [SerializeField] private DropZone _dropZone;
    [SerializeField] private HandController _handController;
    [SerializeField] private CustomButton _actionButton;
    [SerializeField] private Transform _outOfBoundsPos;
    [SerializeField] private InfosButton _infosButton;
    [SerializeField] private CustomButton _rulesButton;
    [SerializeField] private GameObject _rulesPanel;

    private Color _enabled => GameManager.instance._colorPalette[1];
    private Color _disabled => GameManager.instance._colorPalette[5];
    private Color _actionButtonColor => GameManager.instance.connectionManager.MyTurn ? _enabled : _disabled;

    public DropZone DropZone => _dropZone;
    public HandController HandController => _handController;
    public Vector2 OutOfBoundsPos => _outOfBoundsPos.position;


    public void Setup(params object[] parameters) {
        _rulesButton.Setup(ShowRules, GameManager.instance._colorPalette[2]);
        _rulesPanel.SetActive(false);
    }

    public void SetTheme(string theme, string description) {
        _tmproTheme.text = theme;
        _infosButton.SetTheme(theme, description);
        _actionButton.Setup(Contest, _actionButtonColor, "Contest");
    }

    public void SetActionButton(bool interactable) {
        _actionButton.SetInteractable(interactable);
        _actionButton.SetColor(_actionButtonColor);
    }

    public async Task ShowResult(string result) {
        List<DataContestResult> results = new List<DataContestResult>();
        string[] res = result.Split(";");

        foreach (string s in res) {
            string[] ss = s.Split('=');
            results.Add(new DataContestResult(bool.Parse(ss[0].ToLower()), ss[1]));
        }

        await _dropZone.Reveal(results);

        _actionButton.Setup(NextRound, _actionButtonColor, "Next round");
    }

    public void OnGameEnded() {
        Action onClick = () => GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageReturnToLobby);
        _actionButton.Setup(onClick, _actionButtonColor, "Return to lobby");
    }

    public void Contest() {
        GameManager.instance.loading.Load(true);
        GameManager.instance.inputManager.Enable(InteractableTags.RulesButton);
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageContest);
    }

    public void NextRound() {
        GameManager.instance.inputManager.Enable(InteractableTags.RulesButton);
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageNextRound);
    }

    public async void ClearBoard() {
        await _dropZone.Clear();
        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessageClearBoard);
    }

    public void ShowRules() {
        _rulesPanel.SetActive(true);
        _rulesPanel.GetComponent<RulesPanel>().Setup(_rulesButton);
        GameManager.instance.inputManager.Enable(InteractableTags.RulesButton);
    }
}
