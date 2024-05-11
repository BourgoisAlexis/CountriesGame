using DG.Tweening;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Card : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _tmproID;
    [SerializeField] private GameObject _visual;

    private Canvas _canvas;
    private DataCard _datas;
    private CardController _controller;

    public DataCard Data => _datas;

    public void Setup(DataCard datas, CardController controller) {
        _datas = datas;
        _controller = controller;

        _canvas = GetComponent<Canvas>();
        _canvas.overrideSorting = true;

        _tmproID.text = _datas.label;
    }

    public void UpdateSorting(int index) {
        _canvas.sortingOrder = index;
    }

    private void Update() {
        if (_controller != null) {
            transform.position = Vector2.Lerp(transform.position, _controller.transform.position, Utils.AnimSpeedBasedOnTime);
            float xDiff = transform.position.x - _controller.transform.position.x;
            transform.eulerAngles = new Vector3(0, 0, Utils.AnimSpeedBasedOnTime * xDiff);
        }
    }

    public void ShowShadow() {
        _visual.transform.DOLocalMove(new Vector2(-20, 20), AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public void HideShadow() {
        _visual.transform.DOLocalMove(Vector2.zero, AppConst.animDuration).SetEase(AppConst.animEase);
    }

    public async void Reveal(DataContestResult result) {
        ShowShadow();
        await transform.DOScaleX(0, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        _tmproID.text = result.value;
        _visual.GetComponent<Image>().color = result.result ? GameManager.instance._colorPalette[5] : GameManager.instance._colorPalette[6];
        await transform.DOScaleX(1, AppConst.animDuration).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        _controller.SetLabel(_datas.label);
        HideShadow();
    }

    private async void DownloadImage(string MediaUrl) {
        //UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        //yield return request.SendWebRequest();
        //if (request.isNetworkError || request.isHttpError)
        //    Debug.Log(request.error);
        //else
        //    YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}
