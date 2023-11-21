using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetRequestUI : MonoBehaviour {
    internal Requests request;
    RequestUIManager manager;

    private void Start() {
        manager = GameObject.FindObjectOfType<RequestUIManager>();
    }


    public void SetUI() {
        for (int i = 0; i < 5; i++) {
            manager.x[i].text = request.x[i].ToString();
            manager.y[i].text = request.y[i].ToString();
        }
        manager.y[request.index].text = "";

        manager.displayRequestUI = true;
    }
}
