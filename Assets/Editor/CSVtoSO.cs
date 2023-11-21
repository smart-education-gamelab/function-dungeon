using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.Localization.Settings;
using System;

public class CSVtoSO {
    private static string questionCSVENPath = "/Editor/VragenEngels.csv";
    private static string questionCSVNLPath = "/Editor/VragenNederlands.csv";

    //[MenuItem("Utilities/Generate Feedback")]
    //public static void GenerateFeedback()
    //{
    //    string[] allLines = File.ReadAllLines(Application.dataPath + questionCSVPath);
    //    for (int i = 0; i < allLines.Length; i++)
    //    {
    //        Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();
    //        dialogue.content = new Content[2];
    //        AssetDatabase.CreateAsset(dialogue, $"Assets/Scriptable Objects/Feedback/{i}.asset");
    //    }
    //}

    [MenuItem("Utilities/Generate Questions")]
    public static void GenerateQuestions() {
        string[] allLinesNL = File.ReadAllLines(Application.dataPath + questionCSVNLPath);
        string[] allLinesEN = File.ReadAllLines(Application.dataPath + questionCSVENPath);
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Sprites/Math/Sonia" });
        string[] guids2 = AssetDatabase.FindAssets("t:Dialogue", new[] { "Assets/Scriptable Objects/Feedback" });

        //Debug.Log(guids.Length);

        //Debug.Log(allLinesNL.Length);
        //Debug.Log(allLinesEN.Length);

        //Debug.Log(allLinesEN[0]);
        //Debug.Log(allLinesEN[allLinesEN.Length - 1]);
        //Debug.Log(allLinesNL[0]);
        //Debug.Log(allLinesNL[allLinesNL.Length - 1]);

        //for (int i = 0; i < 92; i++) {
        //    string[] splitDataNL = allLinesNL[i].Split(',');
        //    string[] splitDataEN = allLinesEN[i].Split(',');
        //    if (!splitDataNL[0].Equals(splitDataEN[0])) Debug.Log(i + "\n" + splitDataNL[0] + "\n" + splitDataEN[0]);
        //}
        for (int i = 0; i < allLinesNL.Length; i++)//(string s in allLines)
        {
            string[] splitDataNL = allLinesNL[i].Split(',');
            string[] splitDataEN = allLinesEN[i].Split(',');

            for (int j = 0; j < splitDataNL.Length; j++) {
                splitDataNL[j] = splitDataNL[j].Replace(";", ",");
                allLinesEN[j] = allLinesEN[j].Replace(";", ",");
            }

            Math math = ScriptableObject.CreateInstance<Math>();
            math.id = splitDataNL[0];
            MathText textNL = new MathText();
            textNL.question = splitDataNL[1];
            textNL.correct = splitDataNL[2];
            textNL.wrong1 = splitDataNL[3];
            textNL.wrong2 = splitDataNL[4];
            textNL.wrong3 = splitDataNL[5];
            textNL.feedback = splitDataNL[10];
            textNL.locale = LocalizationSettings.AvailableLocales.Locales[1];
            math.text.Add(textNL);

            MathText textEN = new MathText();
            textEN.question = splitDataEN[1];
            textEN.correct = splitDataEN[2];
            textEN.wrong1 = splitDataEN[3];
            textEN.wrong2 = splitDataEN[4];
            textEN.wrong3 = splitDataEN[5];
            textEN.feedback = splitDataEN[10];
            textEN.locale = LocalizationSettings.AvailableLocales.Locales[0];
            math.text.Add(textEN);

            math.section = splitDataNL[6];
            math.variation = splitDataNL[7];
            math.learningGoal = splitDataNL[8];
            math.dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guids2[0]));

            for (int n = 0; n < guids.Length; n++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[n]);
                if (splitDataNL[9] == AssetDatabase.LoadAssetAtPath<Sprite>(path).name) {
                    //Debug.Log(AssetDatabase.LoadAssetAtPath<Sprite>(path).name);
                    math.image = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    break;
                }
            }
            AssetDatabase.CreateAsset(math, $"Assets/Scriptable Objects/Math/Imported/{math.id}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
