using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ViewManager : MonoBehaviour, Imanager {
    [SerializeField] private TransitionPanel _transitionPanel;
    [SerializeField] private List<GameObject> _views = new List<GameObject>();
    [SerializeField] private CanvasGroup _splashScreen;

    private int _currentViewIndex = -1;


    public async void Setup(params object[] parameters) {
        GameManager.instance.inputManager.Disable(true);

        foreach (GameObject view in _views)
            view.SetActive(false);

        _transitionPanel.Setup();
        await GameManager.instance.TaskWithDelay(GameManager.instance.debugMode ? 0 : 3);
        await _splashScreen.DOFade(0, 1f).SetEase(AppConst.animEase).AsyncWaitForCompletion();
        _splashScreen.gameObject.SetActive(false);

        ShowView((int)parameters[0]);
        GameManager.instance.inputManager.Disable(false);
    }

    public async void ShowView(int index) {
        if (_currentViewIndex == index)
            return;

        if (_currentViewIndex > -1)
            await HideCurrentView();

        _currentViewIndex = index;
        _views[_currentViewIndex].gameObject.SetActive(true);
        await GameManager.instance.TaskWithDelay(1);
        await _transitionPanel.Out();
    }

    public async Task HideCurrentView() {
        await _transitionPanel.In();
        _views[_currentViewIndex].gameObject.SetActive(false);
    }
}
