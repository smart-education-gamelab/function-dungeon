using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public void PlayLevel(string level) {
        Globals.SceneManager.SetScene(level);
        Globals.UIManager.CloseMenu(1.0f);
    }

    public void GenerateLevel() {
        Globals.UIManager.SetMenu("LevelGenerator");
    }
}
