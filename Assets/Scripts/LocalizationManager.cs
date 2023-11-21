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
    public ExerciseList questionList;

    public IEnumerator Start() {
        yield return LocalizationSettings.InitializationOperation;
        foreach (var question in questionList.list) {
            Debug.Log(question.id);
            foreach (MathText text in question.text) {
                var table = LocalizationSettings.StringDatabase.GetTableAsync("Questions", text.locale);
                var stringTable = table.Result;
                stringTable.AddEntry(question.GetQuestionLocalizationKey(), text.question);
                stringTable.AddEntry(question.GetCorrectLocalizationKey(), text.correct);
                stringTable.AddEntry(question.GetWrong1LocalizationKey(), text.wrong1);
                stringTable.AddEntry(question.GetWrong2LocalizationKey(), text.wrong2);
                stringTable.AddEntry(question.GetWrong3LocalizationKey(), text.wrong3);
                stringTable.AddEntry(question.GetFeedbackLocalizationKey(), text.feedback);
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