using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardLock
{
    public static void Lock(string id)
    {
        PlayerPrefs.SetInt("Entity_" + id, 0);
    }

    public static void UnLock(string id)
    {
        PlayerPrefs.SetInt("Entity_" + id, 1);
    }
}

public static class LevelLock
{
    public static void Lock(string id)
    {
        PlayerPrefs.SetInt("Level_" + id, 0);
    }

    public static void UnLock(string id)
    {
        PlayerPrefs.SetInt("Level_" + id, 0);
    }
}
