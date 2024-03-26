using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using FDLocalization = System.Collections.Generic.Dictionary<string, string>;

/*Used to define where localizations are stored.
 * DEFAULT contains all in-game text for menus, NPCs, the tutorial, etc.
 * QUESTIONS is initialized when the game loads using the predefined list of questions
 */
public enum LocalizationTable {
    DEFAULT,
    QUESTIONS
}

public class LocalizationManager : MonoBehaviour {
    public QuestionList questionList;
    private static Dictionary<LocaleIdentifier, FDLocalization> localizations = new Dictionary<LocaleIdentifier, FDLocalization>();

    public void Start() {
        //yield return LocalizationSettings.InitializationOperation;
        int index = 0;
        foreach (var question in questionList.questions) {
            question.uniqueIdentifier = index;
            index++;
            foreach (QuestionText text in question.text) {

                if (!localizations.ContainsKey(text.locale.Identifier)) {
                    localizations[text.locale.Identifier] = new FDLocalization();
                }

                localizations[text.locale.Identifier].Add(question.GetQuestionLocalizationKey(), text.question);
                localizations[text.locale.Identifier].Add(question.GetCorrectLocalizationKey(), text.correct);
                localizations[text.locale.Identifier].Add(question.GetWrong1LocalizationKey(), text.wrong1);
                localizations[text.locale.Identifier].Add(question.GetWrong2LocalizationKey(), text.wrong2);
                localizations[text.locale.Identifier].Add(question.GetWrong3LocalizationKey(), text.wrong3);
                localizations[text.locale.Identifier].Add(question.GetFeedbackLocalizationKey(), text.feedback);

                //var loadOperation = LocalizationSettings.StringDatabase.GetTableAsync("Questions", text.locale);
                //yield return loadOperation;
                //var stringTable = loadOperation.Result;
                //try {
                //    stringTable.AddEntry(question.GetQuestionLocalizationKey(), text.question);
                //    stringTable.AddEntry(question.GetCorrectLocalizationKey(), text.correct);
                //    stringTable.AddEntry(question.GetWrong1LocalizationKey(), text.wrong1);
                //    stringTable.AddEntry(question.GetWrong2LocalizationKey(), text.wrong2);
                //    stringTable.AddEntry(question.GetWrong3LocalizationKey(), text.wrong3);
                //    stringTable.AddEntry(question.GetFeedbackLocalizationKey(), text.feedback);
                //} catch (Exception ex) {
                //    Debug.Log(ex);
                //}
            }
        }
    }


    public static string Localize(string key, LocalizationTable table) {
        if (table == LocalizationTable.QUESTIONS) {
            return localizations[LocalizationSettings.SelectedLocale.Identifier][key];
        }
        LocalizedString localized = new LocalizedString();
        switch (table) {
            case LocalizationTable.DEFAULT: localized.TableReference = "Default"; break;
            //case LocalizationTable.QUESTIONS: localized.TableReference = "Questions"; break;
        }
        localized.TableEntryReference = key;
        return localized.GetLocalizedString();
    }
}