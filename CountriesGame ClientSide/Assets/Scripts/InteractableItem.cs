using UnityEngine;

public class InteractableItem : MonoBehaviour {
    public InteractableTags interactableTag;

    protected bool _interactable;
    protected bool _hovered;

    public virtual void SetInteractable(bool interactable) {
        _interactable = interactable;
    }

    public virtual void PointerEnter() {

    }

    public virtual void PointerExit() {

    }

    public virtual void PointerDown() {

    }

    public virtual void PointerUp() {

    }
}
