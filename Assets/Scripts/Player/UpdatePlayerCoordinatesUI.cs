using UnityEngine;
using TMPro;

public class UpdatePlayerCoordinatesUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coText;

    void Update()
    {
        coText.text = "(" + Mathf.Round(Globals.PlayerController.transform.position.x) + " , " + Mathf.Round(Globals.PlayerController.transform.position.y) + ")";
    }
}
