using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Volume volume;

    [Serializable]
    public class UIScreen {
        public string name;
        public CanvasGroup panel;
        [NonSerialized] public Tween tween;
    }

    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private UIScreen[] screens;
    [SerializeField] private float fadeTime = 0.125f;
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private Image loadingIcon;
    private bool blackScreenActive = false;
    private string firstScreenAfterLoading = "";
    private Tween blackScreenTween, mainFade;
    private UIScreen activeScreen;
    private bool initialized = false;

    void Awake() {
        foreach (var screen in screens) {
            if (screen.panel != null) {
                screen.panel.blocksRaycasts = false;
                screen.panel.alpha = 0.0f;
            }
        }

        blackScreen.gameObject.SetActive(true);
        CloseMenu();
        BlackScreenFadeIn(0.0f, true);
        Time.timeScale = 0.0f;
        StartCoroutine(WaitForInitialLoadingScreen());
    }

    private IEnumerator WaitForInitialLoadingScreen() {
        yield return LocalizationSettings.InitializationOperation;
        var tableOp = LocalizationSettings.StringDatabase.GetAllTables();
        yield return tableOp;

        BlackScreenFadeOut(1.0f);
        Time.timeScale = 1.0f;
        initialized = true;
        if (firstScreenAfterLoading.Length != 0) SetMenu(firstScreenAfterLoading);
    }

    public Tween BlackScreenFadeIn(float duration, bool loadingIconActive) {
        blackScreenActive = true;
        loadingIcon.gameObject.SetActive(loadingIconActive);
        blackScreenTween?.Kill();
        return DOTween.Sequence().Append(DOTween.To(() => blackScreen.alpha, x => blackScreen.alpha = x, 1.0f, duration).OnComplete(() => {
            blackScreen.blocksRaycasts = false;
            blackScreen.interactable = false;
        })).SetUpdate(true);
    }

    public Tween BlackScreenFadeOut(float duration) {
        blackScreenTween?.Kill();
        blackScreen.blocksRaycasts = false;
        blackScreen.interactable = false;
        return DOTween.To(() => blackScreen.alpha, x => blackScreen.alpha = x, 0.0f, duration).SetUpdate(true)
            .OnComplete(()=> blackScreenActive = false);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void CloseMenu() {
        CloseMenu(fadeTime);
    }

    public void CloseMenu(float fadeTime) {
        if (activeScreen != null) {
            FadePanelOut(activeScreen, fadeTime);
            activeScreen = null;
        }

        FadeDOFOut(fadeTime);
        FadeMainPanelOut(fadeTime).OnComplete(() => {
            Time.timeScale = 1.0f;
        });
    }

    public void SetMenu(string name) {
        SetMenu(name, fadeTime);
    }

    public void SetMenu(string name, float time) {
        if (!initialized) {
            firstScreenAfterLoading = name;
            return;
        }
        var screen = screens.First(x => x.name == name);
        if (screen != null) {
            if (activeScreen == screen) return;
            Time.timeScale = 0.0f;
            if (activeScreen == null) FadeMainPanelIn(time);
            if (activeScreen != null) FadePanelOut(activeScreen, time / 2.0f);
            FadeDOFIn(time);
            FadePanelIn(screen, time);
            activeScreen = screen;
        } else Debug.LogError($"UIElement {name} is unknown");
    }

    public string GetActiveMenu() {
        return activeScreen == null ? "" : activeScreen.name;
    }

    public void Update() {
        //bool debugger = (Application.isEditor || Debug.isDebugBuild) && Globals.Debugger.IsOpen();
        if (Input.GetKeyDown(KeyCode.Escape)/* && !debugger*/) {
            TogglePause();
        }
    }

    private void FadePanelIn(UIScreen screen, float time) {
        screen.tween?.Kill();
        screen.panel.blocksRaycasts = true;
        screen.panel.gameObject.SetActive(true);
        if (time == 0.0f) {
            screen.panel.alpha = 1.0f;
        } else {
            screen.tween = DOTween.To(() => screen.panel.alpha, x => screen.panel.alpha = x, 1.0f, time).SetEase(Ease.OutQuint).SetUpdate(true);
        }
    }

    private void FadePanelOut(UIScreen group, float time) {
        group.tween?.Kill();
        group.panel.blocksRaycasts = false;
        if (time == 0.0f) {
            group.panel.alpha = 0.0f;
            group.panel.gameObject.SetActive(false);
        } else {
            group.tween = DOTween.To(() => group.panel.alpha, x => group.panel.alpha = x, 0.0f, time)
                .OnComplete(() => { group.panel.gameObject.SetActive(false); }).SetUpdate(true);
        }
    }

    private void FadeMainPanelIn(float time) {
        mainFade?.Kill();
        mainPanel.blocksRaycasts = true;
        mainPanel.gameObject.SetActive(true);
        FadeDOFIn(time);
        if (time == 0.0f) {
            mainPanel.alpha = 1.0f;
        } else {
            mainFade = DOTween.To(() => mainPanel.alpha, x => mainPanel.alpha = x, 1.0f, time).SetUpdate(true);
        }
    }

    private Tween FadeMainPanelOut(float time) {
        mainFade?.Kill();
        mainPanel.blocksRaycasts = false;
        if (time == 0.0f) {
            mainPanel.alpha = 0.0f;
            mainPanel.gameObject.SetActive(false);
        } else {
            mainFade = DOTween.To(() => mainPanel.alpha, x => mainPanel.alpha = x, 0.0f, time)
                .OnComplete(() => { mainPanel.gameObject.SetActive(false); }).SetEase(Ease.InCirc).SetUpdate(true);
        }

        return mainFade;
    }

    Tween DOFtween;
    private void FadeDOFIn(float time) {
        DOFtween?.Kill();
        if (time == 0.0f) {
            volume.weight = 1.0f;
        } else {
            DOFtween = DOTween.To(() => volume.weight, x => volume.weight = x, 1.0f, time).SetEase(Ease.OutQuart).SetUpdate(true);
        }
    }

    private Tween FadeDOFOut(float time) {
        DOFtween?.Kill();
        if (time == 0.0f) {
            volume.weight = 0.0f;
        } else {
            DOFtween = DOTween.To(() => volume.weight, x => volume.weight = x, 0.0f, time).SetEase(Ease.InQuart).SetUpdate(true);
        }

        return DOFtween;
    }

    public bool IsOpen() {
        return GetActiveMenu().Length != 0;
    }

    private void TogglePause() {
        string active = GetActiveMenu();
        if (active == "" && !blackScreenActive) {
            SetMenu("Pause");
        } else if (active == "Pause") {
            CloseMenu();
        }
    }
}
