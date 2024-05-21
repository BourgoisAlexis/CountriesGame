using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : InteractableItem {
    #region Variables
    private Image _image;
    private TextMeshProUGUI _tmproLabel;
    private Color _basicColor;
    private Color _translucentColor;
    private Color _transparentColor;

    private float _initialY;

    private bool _dragged;
    private bool _draggable;
    private Action _onDragStart;
    private Action _onDragEnd;

    private int baseSortingOrder;

    public bool Dragged => _dragged;
    #endregion

    public Card Card { get; protected set; }

    protected virtual void Awake() {
        _image = GetComponent<Image>();
        _tmproLabel = GetComponentInChildren<TextMeshProUGUI>();

        _initialY = transform.localPosition.y;

        _dragged = false;
        _hovered = false;

        interactableTag = InteractableTags.CardController;
        SetLabel("");
        _tmproLabel.color = _transparentColor;
    }

    public void Setup(Card card, Action onDragStart = null, Action onDragEnd = null) {
        //Color
        _basicColor = GameManager.instance._colorPalette[0];
        _translucentColor = _basicColor;
        _transparentColor = _basicColor;

        _translucentColor.a = 0.3f;
        _transparentColor.a = 0f;
        //Color

        Card = card;
        _onDragStart = onDragStart;
        _onDragEnd = onDragEnd;
        _draggable = true;
    }

    public void DefineNewDefaultPos(Vector2 pos) {
        transform.localPosition = pos;
        _initialY = pos.y;
    }

    public void SetDraggable(bool draggable) {
        _draggable = draggable;
    }

    public void SetLabel(string label) {
        _tmproLabel.text = label;
    }

    public void SetParent(Transform t) {
        transform.SetParent(t);
        transform.localScale = Vector3.one;
        Card.transform.SetParent(t);
        Card.transform.localScale = Vector3.one;
    }

    public override void PointerEnter() {
        if (_dragged)
            return;

        if (CheckForNullref())
            return;

        _image.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _tmproLabel.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);

        transform.DOLocalMoveY(_initialY + 30, AppConst.animDuration).SetEase(AppConst.animEase);
        baseSortingOrder = Card.GetComponent<Canvas>().sortingOrder;
        Card.UpdateSorting(AppConst.sortHover);
        _hovered = true;
    }

    public override void PointerExit() {
        if (_dragged)
            return;

        if (CheckForNullref())
            return;

        _image.DOColor(_translucentColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _tmproLabel.DOColor(_transparentColor, AppConst.animDuration).SetEase(AppConst.animEase);

        transform.DOLocalMoveY(_initialY, AppConst.animDuration).SetEase(AppConst.animEase);
        Card.UpdateSorting(baseSortingOrder);
        _hovered = false;
    }

    public override void PointerDown() {
        if (!_draggable)
            return;

        _dragged = _hovered;
        Card.ShowShadow();
        _onDragStart?.Invoke();
    }

    public override void PointerUp() {
        if (!_draggable)
            return;

        _dragged = false;
        Card.HideShadow();
        _onDragEnd?.Invoke();
    }

    public void DropCard() {
        _dragged = false;
        Card.HideShadow();
    }

    private bool CheckForNullref() {
        if (transform == null || _image == null || _tmproLabel == null) {
            Utils.Log(this, "CheckForNullRef", "one or more ref are missing");
            return true;
        }

        return false;
    }


    private void OnDrawGizmos() {
        Gizmos.color = _dragged ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
