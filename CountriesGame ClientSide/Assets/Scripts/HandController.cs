public class HandController : CardGrouper {
    protected override void SetupCreatedCard(CardController controller, Card card) {
        controller.Setup(card, () => OnDragStart(controller), () => OnDragEnd(controller));
    }

    public async void DrawCard(DataCard datas) {
        CardController controller = CreateCard(datas);
        await GameManager.instance.TaskWithDelay(_taskDelay);
        InsertCard(0, controller);
    }

    private void OnDragStart(CardController controller) {
        if (!_cardControllers.Contains(controller))
            return;

        GameManager.instance.inputManager.StartDragCard(controller);

        _cardControllers.Remove(controller);
        controller.Card.UpdateSorting(AppConst.sortDrag);
        PlaceCards();
    }

    private void OnDragEnd(CardController target) {
        if (_cardControllers.Contains(target))
            return;

        _cardControllers.Add(target);
        PlaceCards();
    }
}
