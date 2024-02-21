using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/*Used to define where localizations are stored.
 * DEFAULT contains all in-game text for menus, NPCs, the tutorial, etc.
 * QUESTIONS is initialized when the game loads using the predefined list of questions
 */
public enum LocalizationTable {
    DEFAULT,
    QUESTIONS
}

public class LocalizationManager : MonoBehaviour {
    public IEnumerator Start() {
        yield return LocalizationSettings.InitializationOperation;
        int index = 0;
        foreach (var question in Globals.MathManager.GetQuestionList().questions) {
            question.uniqueIdentifier = index;
            index++;
            foreach (QuestionText text in question.text) {
                var stringTable = LocalizationSettings.StringDatabase.GetTableAsync("Questions", text.locale).WaitForCompletion();
                try {
                    print(question.GetQuestionLocalizationKey());
                    stringTable.AddEntry(question.GetQuestionLocalizationKey(), text.question);
                    stringTable.AddEntry(question.GetCorrectLocalizationKey(), text.correct);
                    stringTable.AddEntry(question.GetWrong1LocalizationKey(), text.wrong1);
                    stringTable.AddEntry(question.GetWrong2LocalizationKey(), text.wrong2);
                    stringTable.AddEntry(question.GetWrong3LocalizationKey(), text.wrong3);
                    stringTable.AddEntry(question.GetFeedbackLocalizationKey(), text.feedback);
                } catch (Exception ex) {
                    Debug.Log(ex);
                }

            }
        }
    }

    public static string Localize(string key, LocalizationTable table) {
        LocalizedString localized = new LocalizedString();
        switch (table) {
            case LocalizationTable.DEFAULT: localized.TableReference = "Default"; break;
            case LocalizationTable.QUESTIONS: localized.TableReference = "Questions"; break;
        }
        localized.TableEntryReference = key;
        return localized.GetLocalizedString();
    }
}