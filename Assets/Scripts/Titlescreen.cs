using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour
{
    public void OnLevel_Press(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
