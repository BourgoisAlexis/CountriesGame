using UnityEngine;

public class Loading : MonoBehaviour {
    [SerializeField] private GameObject _visual;
    [SerializeField] private GameObject _icon;

    public void Load(bool loading) {
        _visual.SetActive(loading);
    }
}
