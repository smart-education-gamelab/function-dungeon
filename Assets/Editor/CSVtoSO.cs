using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.Localization.Settings;

public class CSVtoSO {
    private static string questionCSVPath = "/Editor/wiskunde opdrachten.csv";

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
        string[] allLines = File.ReadAllLines(Application.dataPath + questionCSVPath);
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Sprites/Math/Sonia" });
        string[] guids2 = AssetDatabase.FindAssets("t:Dialogue", new[] { "Assets/Scriptable Objects/Feedback" });

        Debug.Log(guids.Length);

        for (int i = 0; i < allLines.Length; i++)//(string s in allLines)
        {
            string[] splitData = allLines[i].Split(';');
            Math math = ScriptableObject.CreateInstance<Math>();
            math.id = splitData[0];
            MathText text = new MathText();
            text.question = splitData[1];
            text.correct = splitData[2];
            text.wrong1 = splitData[3];
            text.wrong2 = splitData[4];
            text.wrong3 = splitData[5];
            text.feedback = splitData[10];
            text.locale = LocalizationSettings.ProjectLocale;
            Debug.Log(LocalizationSettings.ProjectLocale);

            math.text.Add(text);

            math.section = splitData[6];
            math.variation = splitData[7];
            math.learningGoal = splitData[8];
            math.dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guids2[0]));

            for (int n = 0; n < guids.Length; n++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[n]);
                if (splitData[9] == AssetDatabase.LoadAssetAtPath<Sprite>(path).name) {
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
