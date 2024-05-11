using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : Imanager {
    private List<InteractableItem> _hoveredItems = new List<InteractableItem>();
    private CardController _draggedCard;
    private bool _enabled;
    private bool _buttonsAllowed;

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
        DetectInteractables();

        if (!_enabled && !_buttonsAllowed)
            return;

        if (Input.GetMouseButtonDown(0))
            foreach (InteractableItem item in _hoveredItems) {
                if (!_enabled && _buttonsAllowed)
                    if (item.Tag != AppConst.tagButton)
                        continue;

                item.PointerDown();
            }

        if (Input.GetMouseButtonUp(0)) {
            foreach (InteractableItem item in _hoveredItems) {
                if (!_enabled && _buttonsAllowed)
                    if (item.Tag != AppConst.tagButton)
                        continue;

                if (item != _draggedCard)
                    item.PointerUp();
            }

            if (_draggedCard)
                DropCard();
        }

        if (!_enabled)
            return;

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

                if (currentHovered.Find(x => !string.IsNullOrEmpty(x.Tag) && x.Tag == item.Tag))
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

    public void Disable(bool buttonsAllowed = false) {
        GameManager.instance.boardManager.SetActionButton(buttonsAllowed);

        _buttonsAllowed = buttonsAllowed;
        _enabled = false;
    }

    public void Enable() {
        GameManager.instance.boardManager.SetActionButton(true);

        _buttonsAllowed = true;
        _enabled = true;
    }
}
