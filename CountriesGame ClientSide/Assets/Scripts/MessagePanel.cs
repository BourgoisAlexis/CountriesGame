using DG.Tweening;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : InteractableItem {
    [SerializeField] private TextMeshProUGUI _tmproMessage;

    private Vector2 _initMin;
    private Vector2 _initMax;
    private Image _image;
    private Color _basicColor;
    private Color _translucentColor;

    [SerializeField] private string _currentLink;

    private RectTransform _rectTransform => transform as RectTransform;

    private void Awake() {
        _initMin = _rectTransform.anchorMin;
        _initMax = _rectTransform.anchorMax;

        _image = GetComponent<Image>();
        _basicColor = GameManager.instance._colorPalette[6];
        _translucentColor = _basicColor;
        _translucentColor.a = 0.7f;

        _image.color = _translucentColor;

        _currentLink = string.Empty;
    }

    public void Show(string message) {
        _currentLink = string.Empty;

        if (message.Contains("<link>")) {
            Match m = Regex.Match(message, " <link>(.*?)</link>");
            message = message.Replace(m.ToString(), "");
            _currentLink = m.ToString().Replace("<link>", "");
            _currentLink = _currentLink.Replace("</link>", "");
        }

        _tmproMessage.text = message;
        _rectTransform.DOAnchorMin(_initMin, AppConst.animDuration).SetEase(AppConst.animEase);
        _rectTransform.DOAnchorMax(_initMax, AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public async void Hide() {
        while (_hovered)
            await Task.Yield();

        _rectTransform.DOAnchorMin(_initMin + Vector2.up * 1, AppConst.animDuration).SetEase(AppConst.animEase);
        _rectTransform.DOAnchorMax(_initMax + Vector2.up * 1, AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public async void ShowTemporary(string message) {
        Show(message);
        await GameManager.instance.TaskWithDelay(1);
        Hide();
    }

    public async void ShowError(string message) {
        Show(message);
        await GameManager.instance.TaskWithDelay(6);
        Hide();
    }

    public override void PointerEnter() {
        _image.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _hovered = true;
    }

    public override void PointerExit() {
        _image.DOColor(_translucentColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _hovered = false;
    }

    public override void PointerDown() {
        if (string.IsNullOrEmpty(_currentLink))
            return;

        Application.OpenURL(_currentLink);
    }
}
