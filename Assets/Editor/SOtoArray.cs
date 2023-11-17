using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SOtoArray : MonoBehaviour
{
    [MenuItem("Utilities/Questions to Array")]
    public static void ExercisesToArray()
    {
        Math[] questions;
        string[] guids = AssetDatabase.FindAssets("t:Math", new[] { "Assets/Scriptable Objects/Math/Imported" });
        string[] guids2 = AssetDatabase.FindAssets("t:ExerciseList", new[] { "Assets/Scriptable Objects/Math/ExerciseList" });
        int count = guids.Length;
        Debug.Log("SO to array start before - count guids Math: "); // AB
        Debug.Log(count); // AB
        Debug.Log("SO to array start before - count guids2 ExerciseList: "); // AB
        Debug.Log(guids2.Length); // AB
        questions = new Math[count];
        for (int n = 0; n < count; n++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[n]);
            questions[n] = AssetDatabase.LoadAssetAtPath<Math>(path);
        }

        ExerciseList list = AssetDatabase.LoadAssetAtPath<ExerciseList>(AssetDatabase.GUIDToAssetPath(guids2[0]));
        list.list = questions;
        AssetDatabase.SaveAssets();
        Debug.Log("SO to array done"); // AB
    }
}
