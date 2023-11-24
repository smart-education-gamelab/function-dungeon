using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenUI : MonoBehaviour {
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI helpedTxt;
    public TextMeshProUGUI wrongTxt;

    public void OnPress_Continue() {
        FindObjectOfType<EndScreen>().displayEndScreen = false;
        Globals.UIManager.SetMenu("Main");
    }
}
