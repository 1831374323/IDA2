using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainScene,
        LoadScene,
        IllustratehandBook,
        LevelChose
    }
    public static Scene targetScene;//Ŀ�곡��


    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;//����

        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
