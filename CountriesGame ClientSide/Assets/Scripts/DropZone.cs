using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : CardGrouper {
    #region Variables
    public CardController fakeController;

    private Image _image;
    private Color _basicColor;
    private Color _highlightColor;
    #endregion

    public Card CardVisual { get; private set; }

    protected override void Awake() {
        base.Awake();

        _image = GetComponent<Image>();
        _basicColor = _image.color;
        _highlightColor = _basicColor;
        _highlightColor.a = 1f;

        _hovered = false;

        Tag = AppConst.tagDropZone;
    }

    protected override void SetupCreatedCard(CardController controller, Card card) {
        base.SetupCreatedCard(controller, card);
        controller.SetDraggable(false);
    }

    protected override bool IsValidController(CardController controller) {
        return controller != fakeController;
    }

    public void PlayCard(CardController controller) {
        if (controller == null)
            return;

        if (_cardControllers.Contains(controller))
            return;

        controller.DropCard();

        int index = _cardControllers.IndexOf(fakeController);
        if (index < 0)
            index = 0;

        _cardControllers[index] = controller;

        if (controller != fakeController)
            controller.SetParent(transform);

        controller.DefineNewDefaultPos(Vector2.zero);
        controller.SetDraggable(false);

        RemoveFakeController();

        GameManager.instance.connectionManager.SendMessage(AppConst.playerMessagePlayCard, index, controller.Card.Data.id);
    }

    public void TryToInsert() {
        if (_cardControllers.Count < 1) {
            SortList(0);
            return;
        }

        Vector2 pos = Input.mousePosition;

        int fakeDestination = _cardControllers.Count - 1;

        for (int i = 0; i < _cardControllers.Count; i++) {
            Vector2 p = _cardControllers[i].transform.position;

            if (pos.x < p.x) {
                fakeDestination = i;
                break;
            }
        }

        SortList(fakeDestination);

        PlaceCards();
    }

    public async void InsertCard(int index, DataCard datas) {
        CardController controller = CreateCard(datas);
        await GameManager.instance.TaskWithDelay(_taskDelay);
        InsertCard(index, controller);
    }

    private void SortList(int index) {
        if (_cardControllers.IndexOf(fakeController) == index)
            return;

        List<CardController> before = new List<CardController>();
        List<CardController> after = new List<CardController>();
        int i = 0;

        foreach (CardController controller in _cardControllers) {
            if (controller == fakeController)
                continue;

            if (i < index)
                before.Add(controller);
            else if (i >= index)
                after.Add(controller);

            i++;
        }

        _cardControllers.Clear();

        _cardControllers.AddRange(before);
        _cardControllers.Add(fakeController);
        _cardControllers.AddRange(after);
    }

    private void RemoveFakeController() {
        if (_cardControllers.Contains(fakeController))
            _cardControllers.Remove(fakeController);

        fakeController.transform.position = GameManager.instance.boardManager.OutOfBoundsPos;
        PlaceCards();
    }

    public async void Reveal(List<DataContestResult> results) {
        for (int i = 0; i < results.Count; i++) {
            _cardControllers[i].Card.Reveal(results[i]);
            await GameManager.instance.TaskWithDelay(_taskDelay);
        }
    }


    public override void PointerEnter() {
        _image.DOColor(_highlightColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _hovered = true;
    }

    public override void PointerExit() {
        RemoveFakeController();

        _image.DOColor(_basicColor, AppConst.animDuration).SetEase(AppConst.animEase);
        _hovered = false;
    }


    private void OnDrawGizmos() {
        Gizmos.color = _hovered ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}
