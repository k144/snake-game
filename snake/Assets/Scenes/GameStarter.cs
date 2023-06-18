using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (!Input.anyKeyDown) return;
        SceneManager.LoadScene("Snake");
    }
}
