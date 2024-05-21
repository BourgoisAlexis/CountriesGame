using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfosButton : InteractableItem {
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private TextMeshProUGUI _tmproTheme;
    [SerializeField] private TextMeshProUGUI _tmproDescription;

    private Image _image;
    private Color _basicColor;
    private Color _light;

    private void Awake() {
        _image = GetComponent<Image>();
        _canvas.alpha = 0;
        _canvas.gameObject.SetActive(false);
        _interactable = true;
        interactableTag = InteractableTags.RulesButton;
    }

    private void Start() {
        _basicColor = GameManager.instance._colorPalette[2];
        _light = GameManager.instance._colorPalette[0];

        _image.color = _basicColor;
    }

    public void SetTheme(string theme, string description) {
        _tmproTheme.text = theme;
        _tmproDescription.text = description;
    }

    public override void PointerEnter() {
        _image.DOColor(_light, AppConst.animDuration).SetEase(AppConst.animEase);
        _canvas.alpha = 0;
        _canvas.gameObject.SetActive(true);
        _canvas.DOFade(1, AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public override async void PointerExit() {
        _image.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        await _canvas.DOFade(0, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        _canvas.gameObject.SetActive(false);
    }
}
