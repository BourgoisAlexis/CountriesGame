using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct RulesPage {
    [TextArea(1, 10)] public string content;
}

public class RulesPanel : MonoBehaviour, Imanager {
    [SerializeField] private GameObject _pagePrefab;
    [SerializeField] private List<RulesPage> _pages;
    [SerializeField] private Transform _footer;
    [SerializeField] private TextMeshProUGUI _tmproContent;
    [SerializeField] private CustomButton _closeButton;
    [SerializeField] private CustomButton _previousButton;
    [SerializeField] private CustomButton _nextButton;

    private int _currentPageIndex;
    private List<Image> _paginationElements = new List<Image>();
    private bool _setuped = false;
    private CustomButton _rulesButton;

    public void Setup(params object[] parameters) {
        if (!_setuped) {
            _previousButton.Setup(Previous, GameManager.instance._colorPalette[2]);
            _nextButton.Setup(Next, GameManager.instance._colorPalette[2]);
            _closeButton.Setup(Close, GameManager.instance._colorPalette[2]);

            foreach (RulesPage page in _pages) {
                GameObject instantiatedObj = Instantiate(_pagePrefab, _footer);
                _paginationElements.Add(instantiatedObj.GetComponent<Image>());
            }

            _rulesButton = parameters[0] as CustomButton;

            _setuped = true;
        }

        _currentPageIndex = 0;
        _rulesButton.SetInteractable(false);
        ShowPage();
    }

    public void Next() {
        if (_currentPageIndex == _pages.Count - 1) {
            Close();
            return;
        }

        _currentPageIndex++;
        ShowPage();
    }

    public void Previous() {
        if (_currentPageIndex == 0)
            return;

        _currentPageIndex--;
        ShowPage();
    }

    public void ShowPage() {
        _tmproContent.text = _pages[_currentPageIndex].content;

        for (int i = 0; i < _pages.Count; i++) {
            Image image = _paginationElements[i];
            image.color = i == _currentPageIndex ? GameManager.instance._colorPalette[0] : GameManager.instance._colorPalette[5];
        }
    }

    public void Close() {
        gameObject.SetActive(false);
        _rulesButton.SetInteractable(true);

        if (GameManager.instance.connectionManager.MyTurn)
            GameManager.instance.inputManager.EnableAll();
        else
            GameManager.instance.inputManager.Enable(InteractableTags.RulesButton);
    }
}
