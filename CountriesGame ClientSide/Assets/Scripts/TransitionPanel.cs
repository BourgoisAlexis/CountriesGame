using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TransitionPanel : MonoBehaviour {
    [SerializeField] private List<Transform> _elements = new List<Transform>();

    private Dictionary<Transform, Vector2> _positions = new Dictionary<Transform, Vector2>();

    private float _duration => AppConst.animDuration * 2;

    public void Setup() {
        gameObject.SetActive(true);
        Shuffle();

        foreach (Transform element in _elements) {
            _positions.Add(element, element.localPosition);
            element.SetSiblingIndex(Random.Range(0, _elements.Count));
        }
    }

    public async Task In() {
        foreach (KeyValuePair<Transform, Vector2> pair in _positions) {
            pair.Key.DOLocalMove(pair.Value, _duration).SetEase(Ease.InOutExpo);
            await GameManager.instance.TaskWithDelay(0.01f);
        }

        await GameManager.instance.TaskWithDelay(_duration);
    }

    public async Task Out() {
        foreach (KeyValuePair<Transform, Vector2> pair in _positions) {
            Vector2 panelSize = GetComponent<RectTransform>().rect.size;
            float x = pair.Value.x < 0 ? Random.Range(-panelSize.x * 2, 0) : Random.Range(0, panelSize.x * 2);
            float y = pair.Value.y < 0 ? Random.Range(-panelSize.y * 2, 0) : Random.Range(0, panelSize.y * 2);

            while (Mathf.Abs(x) < panelSize.x && Mathf.Abs(y) < panelSize.y) {
                x = pair.Value.x < 0 ? Random.Range(-panelSize.x * 2, 0) : Random.Range(0, panelSize.x * 2);
                y = pair.Value.y < 0 ? Random.Range(-panelSize.y * 2, 0) : Random.Range(0, panelSize.y * 2);
            }

            Vector2 r = new Vector2(x, y);
            await GameManager.instance.TaskWithDelay(0.01f);
            pair.Key.DOLocalMove(pair.Value + r, _duration).SetEase(Ease.InOutExpo);
        }
    }

    private void Shuffle() {
        System.Random r = new System.Random();
        int n = _elements.Count;
        while (n > 1) {
            n--;
            int k = r.Next(n + 1);
            Transform value = _elements[k];
            _elements[k] = _elements[n];
            _elements[n] = value;
        }
    }
}
