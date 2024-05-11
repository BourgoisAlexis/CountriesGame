using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardGrouper : InteractableItem {
    [SerializeField] protected List<CardController> _cardControllers = new List<CardController>();
    [SerializeField] protected Vector2 _initialSize;
    private Vector2 _targetScale = Vector2.one;
    [SerializeField] private Vector2 _targetSize = Vector2.zero;
    protected float _taskDelay = 0.1f;

    protected RectTransform _rectTransform => transform as RectTransform;

    protected virtual void Awake() {
        _initialSize = _rectTransform.rect.size;
    }

    private void Update() {
        _rectTransform.localScale = Vector2.Lerp(_rectTransform.localScale, _targetScale, Utils.AnimSpeedBasedOnTime * 0.5f);
        _rectTransform.sizeDelta = Vector2.Lerp(_rectTransform.sizeDelta, _targetSize, Utils.AnimSpeedBasedOnTime * 0.5f);
    }

    public CardController CreateCard(DataCard datas) {
        GameManager manager = GameManager.instance;
        GameObject instantiatedObj = Instantiate(manager.prefabCardController);
        CardController controller = instantiatedObj.GetComponent<CardController>();

        instantiatedObj = Instantiate(manager.prefabCard);
        Card card = instantiatedObj.GetComponent<Card>();

        SetupCreatedCard(controller, card);

        controller.transform.position = GameManager.instance.boardManager.OutOfBoundsPos;
        card.transform.position = GameManager.instance.boardManager.OutOfBoundsPos;
        controller.SetParent(transform);
        card.Setup(datas, controller);

        _cardControllers.Add(controller);

        return controller;
    }

    public void InsertCard(int index, CardController controller) {
        if (_cardControllers.Contains(controller))
            _cardControllers.Remove(controller);

        _cardControllers.Insert(index, controller);

        PlaceCards();
    }

    public async void DestroyCard(string id) {
        CardController controller = _cardControllers.Find(x => x.Card.Data.id == id);
        controller.transform.position = GameManager.instance.boardManager.OutOfBoundsPos;
        await GameManager.instance.TaskWithDelay(_taskDelay);

        if (controller != null) {
            _cardControllers.Remove(controller);
            Destroy(controller.Card.gameObject);
            Destroy(controller.gameObject);
        }

        PlaceCards();
    }

    public async Task Clear() {
        List<string> ids = new List<string>();

        foreach (CardController controller in _cardControllers)
            ids.Add(controller.Card.Data.id);

        foreach (string id in ids) {
            DestroyCard(id);
            await GameManager.instance.TaskWithDelay(_taskDelay / 2);
        }
    }

    protected virtual void PlaceCards() {
        if (_cardControllers.Count < 1)
            return;

        RectTransform r = GameManager.instance.prefabCardController.GetComponent<RectTransform>();

        float cardWidth = r.rect.width + 10;
        float neededWidth = cardWidth * _cardControllers.Count;

        if (neededWidth > _initialSize.x) {
            float ratio = _initialSize.x / neededWidth;
            _targetSize = new Vector2(neededWidth - _initialSize.x, (_initialSize.y * (1 / ratio)) - _initialSize.y);
            _targetScale = Vector2.one * ratio;
        }
        else if (_rectTransform.localScale != Vector3.one) {
            _targetSize = Vector3.zero;
            _targetScale = Vector2.one;
        }

        float w = Math.Min(_targetSize.x + _initialSize.x - cardWidth * 2, neededWidth);
        float step = w / (_cardControllers.Count - 1);
        float x = _cardControllers.Count > 1 ? (-w / 2) : 0;
        int sorting = AppConst.sortBase;

        foreach (CardController controller in _cardControllers) {
            controller.transform.localPosition = Vector3.right * x;

            if (IsValidController(controller))
                controller.Card.UpdateSorting(sorting);

            x += step;
            sorting++;
        }
    }


    //Particular cases
    protected virtual void SetupCreatedCard(CardController controller, Card card) {
        controller.Setup(card);
    }

    protected virtual bool IsValidController(CardController controller) {
        return true;
    }
}
