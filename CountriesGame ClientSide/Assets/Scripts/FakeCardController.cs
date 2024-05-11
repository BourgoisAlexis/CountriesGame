using UnityEngine;

public class FakeCardController : CardController {
    [SerializeField] private Card _card;

    protected override void Awake() {
        base.Awake();
        Card = _card;
        _card.Setup(new DataCard("FAKE CARD", "FAKE CARD"), null);
    }
}
