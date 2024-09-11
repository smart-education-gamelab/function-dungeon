using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using LocalizationDict = System.Collections.Generic.Dictionary<string, string>;

/*Used to define where localizations are stored.
 * DEFAULT contains all in-game text for menus, NPCs, the tutorial, etc.
 * QUESTIONS is initialized when the game loads using the predefined list of questions
 */
public enum LocalizationTable {
    DEFAULT,
    QUESTIONS
}

public class LocalizationManager : MonoBehaviour {
    static Dictionary<string, LocalizationDict> localizationTable = new Dictionary<string, LocalizationDict>(); //Apparently we can't really use localization tables directly when using WebGL, so we'll store them in a dictionary instead
    static bool once = true;
    public IEnumerator Start() {
        if (once) {
            once = false;
            yield return LocalizationSettings.InitializationOperation;
            int index = 0;
            foreach (var question in Globals.MathManager.GetQuestionList().questions) {
                question.uniqueIdentifier = index;
                index++;
                foreach (QuestionText text in question.text) {
                    if (!localizationTable.ContainsKey(text.locale.LocaleName)) localizationTable.Add(text.locale.LocaleName, new LocalizationDict());
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetQuestionLocalizationKey(), text.question);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetQuestionLocalizationKey(), text.question);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetCorrectLocalizationKey(), text.correct);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetWrong1LocalizationKey(), text.wrong1);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetWrong2LocalizationKey(), text.wrong2);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetWrong3LocalizationKey(), text.wrong3);
                    localizationTable[text.locale.LocaleName].TryAdd(question.GetFeedbackLocalizationKey(), text.feedback);
                }
            }
        }
    }

    public static string Localize(string key, LocalizationTable table) {
        LocalizedString localized = new LocalizedString();
        if (table == LocalizationTable.DEFAULT) {
            localized.TableReference = "Default";
            localized.TableEntryReference = key;
            return localized.GetLocalizedString();
        } else if (table == LocalizationTable.QUESTIONS) {
            if (localizationTable[LocalizationSettings.SelectedLocale.LocaleName].ContainsKey(key)) {
                return localizationTable[LocalizationSettings.SelectedLocale.LocaleName][key];
            }
        }
        return "Unknown localization: " + key;
    }
}