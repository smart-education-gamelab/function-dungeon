using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
    public bool disableAfterStart = false;
    void Start() {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        if (disableAfterStart) gameObject.SetActive(false);
    }
}
