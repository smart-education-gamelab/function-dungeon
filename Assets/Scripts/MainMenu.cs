using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MainMenu : MonoBehaviour {
    public void PlayLevel(string level) {
        Globals.SceneManager.SetScene(level);
        Globals.UIManager.CloseMenu(1.0f);
    }

    public void GenerateLevel() {
        Globals.UIManager.SetMenu("LevelGenerator");
    }
}
