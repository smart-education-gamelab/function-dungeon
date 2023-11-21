using UnityEngine;
using TMPro;

public class UpdatePlayerCoordinatesUI : MonoBehaviour
{

    private Transform player;
    [SerializeField] private TextMeshProUGUI coText;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        coText.text = "(" + Mathf.Round(player.position.x) + " , " + Mathf.Round(player.position.y) + ")";
    }
}
