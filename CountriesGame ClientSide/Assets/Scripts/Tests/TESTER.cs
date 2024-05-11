using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TESTER : MonoBehaviour {
    private List<CustomButton> _buttons = new List<CustomButton>();

    private void Start() {
        CreateButton("Reset lobby", () => GameManager.instance.lobbyManager.Setup());
        CreateButton("Board view", () => GameManager.instance.viewManager.ShowView(1));
    }

    private void CreateButton(string id, Action action, bool debug = true) {
        if (debug && !GameManager.instance.debugMode)
            return;

        GameObject obj = Instantiate(new GameObject());
        obj.transform.SetParent(transform);
        Image i = obj.AddComponent<Image>();
        CustomButton b = obj.AddComponent<CustomButton>();
        b.Tag = AppConst.tagButton;
        b.Setup(action, Color.black);
        _buttons.Add(b);

        Transform t = obj.transform;
        obj = Instantiate(new GameObject());
        obj.transform.SetParent(t);
        obj.transform.localPosition = Vector3.zero;
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.color = debug ? Color.yellow : Color.white;
        text.text = id;
        text.enableAutoSizing = true;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMax = Vector3.one * 0.9f;
        rect.anchorMin = Vector3.one * 0.1f;
        rect.offsetMin = Vector3.zero;
        rect.offsetMax = Vector3.zero;
    }
}
