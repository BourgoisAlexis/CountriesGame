using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _tmproUserName;
    [SerializeField] private TextMeshProUGUI _tmproCardCount;
    [SerializeField] private GameObject _cardCountLayout;
    private Image _image;
    private int _id;
    private string _name;

    public string Name => _name;

    public void Setup(int playerID, string userName) {
        _image = GetComponentInChildren<Image>();

        _id = playerID;
        _name = userName;
        _tmproUserName.text = userName;
        Lobby();
    }

    public void Highlight(int currentPlayerID, int previousPlayerID) {
        Color color = GameManager.instance._colorPalette[5];

        if (_id == currentPlayerID)
            color = GameManager.instance._colorPalette[1];
        else if (_id == previousPlayerID)
            color = GameManager.instance._colorPalette[2];

        _image.color = color;
    }

    public void Lobby() {
        _image.color = GameManager.instance._colorPalette[2];
        _cardCountLayout.SetActive(false);
    }

    public void UpdateCardCount(int count) {
        _cardCountLayout.SetActive(true);
        _tmproCardCount.text = count.ToString();
    }
}
