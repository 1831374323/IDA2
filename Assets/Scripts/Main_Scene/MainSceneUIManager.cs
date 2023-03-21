using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MainSceneUIManager : MonoBehaviour
{
    [SerializeField] private Button startGame;
    [SerializeField] private Button quitGame;
    [SerializeField] private Button handBook;
    [SerializeField] private Button setting;
    [SerializeField] private GameObject settingCanvas;

    private void Start()
    {
        startGame.onClick.AddListener(() => { Loader.Load(Loader.Scene.LevelChose); });
        quitGame.onClick.AddListener(() => { Application.Quit(); });
        handBook.onClick.AddListener(() => { Loader.Load(Loader.Scene.IllustratehandBook); });
        setting.onClick.AddListener(() => { settingCanvas.SetActive(true); });
        AudioManager.Instance.PlayMusic("BackGround", true);
    }
}
