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
        IllustratehandBook
    }
    public static Scene targetScene;//目标场景


    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;//复制
        SceneManager.LoadScene(Scene.LoadScene.ToString());
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
