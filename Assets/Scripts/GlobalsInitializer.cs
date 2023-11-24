using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsInitializer : MonoBehaviour {
    public Boolean openMainMenu = false;
    public GameObject playerSpawnPoint;

    private void Awake() {
        SceneManager.LoadSceneIfNotActive("Persistent");
    }

    void Start() {
        Globals.Initialize();
        if (openMainMenu) Globals.UIManager.SetMenu("Main");
        if (playerSpawnPoint)
            FindObjectOfType<PlayerController>().transform.position = new Vector3(playerSpawnPoint.transform.position.x, playerSpawnPoint.transform.position.y, 0);
    }
}
