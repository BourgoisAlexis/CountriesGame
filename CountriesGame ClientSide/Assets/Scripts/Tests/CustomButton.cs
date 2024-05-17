using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomButton : InteractableItem {
    private Action _onClick;
    private Color _basicColor;
    private Image _image;
    private TextMeshProUGUI _tmproLabel;

    private Color _light => GameManager.instance._colorPalette[0];
    private Color _dark => GameManager.instance._colorPalette[6];


    private void Awake() {
        _image = GetComponent<Image>();
        _tmproLabel = GetComponentInChildren<TextMeshProUGUI>();
        _interactable = true;
    }

    public void Setup(Action onClick, Color color, string text = "") {
        _onClick = onClick;
        _basicColor = color;

        _image.color = _hovered ? _light : _basicColor;

        if (string.IsNullOrEmpty(text))
            return;

        _tmproLabel.text = text;
        _tmproLabel.color = _hovered ? _dark : _light;
    }


    public void SetColor(Color color) {
        if (color != null)
            _basicColor = color;

        _image.color = _hovered ? _light : _basicColor;
    }

    public override async void PointerDown() {
        if (!_interactable)
            return;

        _onClick?.Invoke();
        _image.color = _light;
        await _image.DOColor(Color.grey, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        _image.DOColor(_hovered ? _light : _basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        if (_tmproLabel)
            _tmproLabel.color = _hovered ? _dark : _light;
    }

    public override void PointerEnter() {
        if (!_interactable)
            return;

        _hovered = true;
        _image.DOColor(_light, AppConst.animDuration).SetEase(AppConst.animEase);
        _tmproLabel?.DOColor(_dark, AppConst.animDuration).SetEase(AppConst.animEase);
        transform.DOScale(1.05f, AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public override void PointerExit() {
        _hovered = false;
        _image.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _tmproLabel?.DOColor(_light, AppConst.animDuration).SetEase(AppConst.animEase);
        transform.DOScale(1f, AppConst.animDuration).SetEase(AppConst.animEase);
    }
}
