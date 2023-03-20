using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EscCanvas : MonoBehaviour
{
    public void BackToMap()
    {
        SceneManager.LoadScene("LevelChose");
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
