using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour {
    int activeLocaleID = 0;
    private void Start() {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
    }

    private bool active = false;

    public void LoopLocale() {
        activeLocaleID++;
        if (activeLocaleID >= LocalizationSettings.AvailableLocales.Locales.Count) activeLocaleID = 0;
        ChangeLocale(activeLocaleID);
    }

    public void ChangeLocale(int localeID) {
        if (!active) {
            activeLocaleID = localeID;
            StartCoroutine(SetLocale(localeID));
        }
    }

    IEnumerator SetLocale(int _localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }
}
