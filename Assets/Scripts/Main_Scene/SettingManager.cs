using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Slider musicslider;//获取两个条
    [SerializeField] private Slider rfxslider;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private Button BackButton;

    private void Start()
    {
        musicslider.value = AudioManager.Instance.MusicVolume;
        rfxslider.value=AudioManager.Instance.SFXVolume;
        BackButton.onClick.AddListener(() => { settingCanvas.SetActive(false); });
    }

}
