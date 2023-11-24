using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Question List", menuName = "Questions List")]
public class QuestionList : ScriptableObject {
    public List<Question> questions;
}

[System.Serializable]
public class QuestionText : ICloneable {
    public Locale locale;
    public string question = "";
    public string correct = "";
    public string wrong1 = "";
    public string wrong2 = "";
    public string wrong3 = "";
    public string feedback = "";

    public object Clone() {
        return MemberwiseClone();
    }
}

public enum QuestionType {
    MULTIPLECHOICE,
    CUSTOM,
    MULTIPLECHOICEGAMIFIED
}

[Serializable]
public class Question : ICloneable {
    [NonSerialized] public int uniqueIdentifier; //Used so multiple questions can share the same name. The variable is assigned during initialization of localizations in the LocalizationManager.
    public string ignoreThis = ""; //This variable is only here so we don't encounter a very strange bug in the editor window. If this string isn't here, the name field of this question will keep losing focus.
    public string name = "";
    public bool enabled = true;
    public QuestionType type = QuestionType.MULTIPLECHOICE;
    public Sprite image;

    public object Clone() {
        Question newQuestion = (Question)MemberwiseClone();
        newQuestion.text = new List<QuestionText>();
        foreach (var t in text) {
            newQuestion.text.Add((QuestionText)t.Clone());
        }
        return newQuestion;
    }

    //Multiple choice
    public List<QuestionText> text = new List<QuestionText>();

    public string section = ""; //The section is now stored in the learning goal level. We no longer use this variable but since it was defined in the excel sheets we'll keep it just in case.
    public string variation = "";
    //public string learningGoal = "";
    public int learningGoalLevel = 0;

    public Dialogue dialogue;

    public bool used = false;

    public string GetQuestionLocalizationKey() {
        return name + uniqueIdentifier + "_QUESTION";
    }
    public string GetCorrectLocalizationKey() {
        return name + uniqueIdentifier + "_CORRECT";
    }
    public string GetWrong1LocalizationKey() {
        return name + uniqueIdentifier + "_WRONG1";
    }
    public string GetWrong2LocalizationKey() {
        return name + uniqueIdentifier + "_WRONG2";
    }
    public string GetWrong3LocalizationKey() {
        return name + uniqueIdentifier + "_WRONG3";
    }
    public string GetFeedbackLocalizationKey() {
        return name + uniqueIdentifier + "_FEEDBACK";
    }

    //Custom
    public GameObject puzzlePrefab;
}