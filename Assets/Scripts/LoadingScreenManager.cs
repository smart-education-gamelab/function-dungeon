using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour {
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private UnityEngine.UI.Image loadingIcon;
    [SerializeField] private TextMeshProUGUI logText;
    private static LoadingScreenManager _Instance;

    private bool blackScreenActive = false;
    private Tween blackScreenTween;

    void Awake() {
        _Instance = this;
        DontDestroyOnLoad(this);
    }

    public static Tween BlackScreenFadeIn(float duration, bool loadingIconActive) {
        if (_Instance == null) return null;
        return _Instance._BlackScreenFadeIn(duration, loadingIconActive);
    }

    private Tween _BlackScreenFadeIn(float duration, bool loadingIconActive) {
        loadingIcon.gameObject.SetActive(loadingIconActive);
        blackScreenTween?.Kill();
        blackScreenActive = true;
        return DOTween.Sequence().Append(DOTween.To(() => blackScreen.alpha, x => blackScreen.alpha = x, 1.0f, duration).OnComplete(() => {
            blackScreen.blocksRaycasts = false;
            blackScreen.interactable = false;
        })).SetUpdate(true);
    }

    public static Tween BlackScreenFadeOut(float duration) {
        if (_Instance == null) return null;
        return _Instance._BlackScreenFadeOut(duration);
    }
    private Tween _BlackScreenFadeOut(float duration) {
        blackScreenTween?.Kill();
        blackScreen.blocksRaycasts = false;
        blackScreen.interactable = false;
        blackScreenActive = false;
        return DOTween.To(() => blackScreen.alpha, x => blackScreen.alpha = x, 0.0f, duration).SetUpdate(true)
            .OnComplete(
                () => {
                });
    }

    public static void SetLogText(string text) {
        if (_Instance == null) return;
        _Instance._SetLogText(text);
    }

    private void _SetLogText(string text) {
        logText.text = text;
    }
}
