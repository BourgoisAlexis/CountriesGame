using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : Imanager {
    private List<InteractableItem> _hoveredItems = new List<InteractableItem>();
    private CardController _draggedCard;
    private bool _enabled;
    private InteractableTags[] _allowedTags = new InteractableTags[] { };

    private GraphicRaycaster[] _raycasters;
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    public void Setup(params object[] parameters) {
        _raycasters = parameters[0] as GraphicRaycaster[];
        _eventSystem = parameters[1] as EventSystem;
        _draggedCard = null;
        _enabled = true;
    }

    public void Update() {
        if (!_enabled)
            return;

        DetectInteractables();

        if (_allowedTags.Length == 0)
            return;

        if (Input.GetMouseButtonDown(0))
            foreach (InteractableItem item in _hoveredItems) {
                if (!IsAllowedTag(item.interactableTag))
                    continue;

                item.PointerDown();
            }

        if (Input.GetMouseButtonUp(0)) {
            foreach (InteractableItem item in _hoveredItems) {
                if (!IsAllowedTag(item.interactableTag))
                    continue;

                if (item != _draggedCard)
                    item.PointerUp();
            }

            if (_draggedCard)
                DropCard();
        }

        if (_draggedCard) {
            _draggedCard.transform.position = Input.mousePosition;
            DropZone zone = GetZone();

            if (zone)
                zone.TryToInsert();
        }
    }

    public void DetectInteractables() {
        _pointerEventData = new PointerEventData(_eventSystem);
        _pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        List<InteractableItem> currentHovered = new List<InteractableItem>();

        foreach (GraphicRaycaster raycaster in _raycasters) {
            raycaster.Raycast(_pointerEventData, results);

            foreach (RaycastResult result in results) {
                InteractableItem item = result.gameObject.GetComponent<InteractableItem>();

                if (item == null)
                    continue;

                if (currentHovered.Find(x => x.interactableTag == item.interactableTag))
                    continue;

                if (!_hoveredItems.Contains(item)) {
                    item.PointerEnter();
                    _hoveredItems.Add(item);
                }

                currentHovered.Add(item);
            }
        }

        List<InteractableItem> notHovered = _hoveredItems.FindAll(x => !currentHovered.Contains(x));

        foreach (InteractableItem item in notHovered)
            item.PointerExit();

        _hoveredItems = currentHovered;
    }

    public void StartDragCard(CardController controller) {
        _draggedCard = controller;
    }

    private void DropCard() {
        DropZone zone = GetZone();

        if (zone)
            zone.PlayCard(_draggedCard);
        else
            _draggedCard.PointerUp();

        _draggedCard = null;
    }

    private DropZone GetZone() {
        DropZone zone = null;

        InteractableItem item = _hoveredItems.Find(x => x.GetComponent<DropZone>() != null);

        if (item)
            zone = item.GetComponent<DropZone>();

        return zone;
    }

    public void Enable(params InteractableTags[] allowedTags) {
        StringBuilder b = new StringBuilder();
        foreach (var tag in allowedTags) {
            b.AppendLine(tag.ToString());
        }
        Debug.Log(b.ToString());

        _allowedTags = allowedTags;
        GameManager.instance.boardManager.SetActionButton(IsAllowedTag(InteractableTags.ActionButton));
    }

    public void EnableAll() {
        Enable(InteractableTags.Default,
            InteractableTags.RulesButton,
            InteractableTags.DropZone,
            InteractableTags.ActionButton,
            InteractableTags.CardController
            );
    }

    private bool IsAllowedTag(InteractableTags tag) {
        foreach (InteractableTags t in _allowedTags)
            if (t.ToString() == tag.ToString())
                return true;

        return false;
    }

    public void Disable(bool disable) {
        _enabled = !disable;
    }
}
