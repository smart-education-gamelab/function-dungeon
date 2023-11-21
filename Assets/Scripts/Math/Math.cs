using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public class MathText {
    //public LocalizedString str;
    public Locale locale;
    public string question;
    public string correct;
    public string wrong1;
    public string wrong2;
    public string wrong3;
    public string feedback;
}

[Serializable]
[CreateAssetMenu(fileName = "New Math", menuName = "Math")]
public class Math : ScriptableObject {
    public string id;
    public Sprite image;

    public List<MathText> text = new List<MathText>();

    public string section;
    public string variation;
    public string learningGoal;

    public Dialogue dialogue;

    public bool used;

    public string GetQuestionLocalizationKey() {
        return id + "_QUESTION";
    }
    public string GetCorrectLocalizationKey() {
        return id + "_CORRECT";
    }
    public string GetWrong1LocalizationKey() {
        return id + "_WRONG1";
    }
    public string GetWrong2LocalizationKey() {
        return id + "_WRONG2";
    }
    public string GetWrong3LocalizationKey() {
        return id + "_WRONG3";
    }
    public string GetFeedbackLocalizationKey() {
        return id + "_FEEDBACK";
    }
}
