using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelButton : MonoBehaviour
{
    public string LevelID;

    public enum LevelState
    {
        unPass,
        Pass,
        PerfectPass
    }

    public LevelState m_state = LevelState.unPass;
    private void Awake()
    {
        m_state = (LevelState)PlayerPrefs.GetInt("Level_" + LevelID, 0);
    }
    public void OnClick()
    {
        SceneManager.LoadScene("Level");
    }
}
