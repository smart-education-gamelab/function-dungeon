using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Restart() {
        Globals.SceneManager.ReloadScene();
        Globals.UIManager.CloseMenu();
    }

    public void MainMenu() {
        //Globals.SceneManager.SetScene("Main");
        Globals.UIManager.SetMenu("Main");
    }
}
