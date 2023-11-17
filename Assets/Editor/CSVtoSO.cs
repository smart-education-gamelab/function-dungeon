using UnityEngine;
using UnityEditor;
using System.IO;

public class CSVtoSO
{
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
    public static void GenerateQuestions()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + questionCSVPath);
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Sprites/Math/Sonia" });
        string[] guids2 = AssetDatabase.FindAssets("t:Dialogue", new[] { "Assets/Scriptable Objects/Feedback" });

        Debug.Log(guids.Length);
        Debug.Log(guids2.Length); // AB
        Debug.Log(allLines.Length); // AB


        for (int i = 0; i < allLines.Length; i++)//(string s in allLines)
        {
            string[] splitData = allLines[i].Split(';');
            Math math = ScriptableObject.CreateInstance<Math>();
            math.id = splitData[0];
            Debug.Log("math.id: "); // AB
            Debug.Log(math.id); // AB

            math.question = splitData[1];
            math.correct = splitData[2];
            math.wrong1 = splitData[3];
            math.wrong2 = splitData[4];
            math.wrong3 = splitData[5];
            math.section = splitData[6];
            math.variation = splitData[7];
            math.learningGoal = splitData[8];
            math.feedback = splitData[10];
            math.dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(AssetDatabase.GUIDToAssetPath(guids2[0]));
                  
            for (int n = 0; n < guids.Length; n++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[n]);
                if (splitData[9] == AssetDatabase.LoadAssetAtPath<Sprite>(path).name)
                {
                    Debug.Log(AssetDatabase.LoadAssetAtPath<Sprite>(path).name);
                    math.image = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    break;
                }
            }
            AssetDatabase.CreateAsset(math, $"Assets/Scriptable Objects/Math/Imported/{math.id}.asset");
        }

        AssetDatabase.SaveAssets();

        Debug.Log("CSV to SO done"); // AB
    }
}
