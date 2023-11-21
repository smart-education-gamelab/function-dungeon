using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsInitializer : MonoBehaviour {
    public Boolean openMainMenu = false;

    private void Awake() {
        SceneManager.LoadSceneIfNotActive("Persistent");
    }

    void Start() {
        Globals.Initialize();
        if (openMainMenu) Globals.UIManager.SetMenu("Main");
    }
}
