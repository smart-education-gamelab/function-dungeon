using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RequestUIManager : MonoBehaviour
{
    public bool displayRequestUI;

    [SerializeField] internal TextMeshProUGUI[] x;
    [SerializeField] internal TextMeshProUGUI[] y;

    public void CloseUI()
    {
        displayRequestUI = false;
    }
}
