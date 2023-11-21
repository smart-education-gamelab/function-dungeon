using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour {
    private static DebugHelper _instance;

    public bool easyQuestions = false;

    void Awake() {
        _instance = this;
    }

    private static bool IsActive() {
        return _instance != null && Application.isEditor;
    }

    public static bool EasyQuestions() {
        return IsActive() && _instance.easyQuestions;
    }
}
