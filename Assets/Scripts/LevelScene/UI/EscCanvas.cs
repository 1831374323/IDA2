using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EscCanvas : MonoBehaviour
{
    [SerializeField] private Slider musicslider;//��ȡ������
    [SerializeField] private Slider rfxslider;
    public void BackToMap()
    {
        SceneManager.LoadScene("LevelChose");
    }

    public void ReStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicslider.GetComponent<Slider>().onValueChanged.AddListener(SetMusicvolume);
            rfxslider.GetComponent<Slider>().onValueChanged.AddListener(SetSFXvolume);
            musicslider.value = AudioManager.Instance.MusicVolume;
            rfxslider.value = AudioManager.Instance.SFXVolume;
        }


    }

    public void SetMusicvolume(float musicVolume)//������ֵ
    {
        AudioManager.Instance.MusicVolume = musicVolume;
    }

    public void SetSFXvolume(float sFXVolume)
    {
        AudioManager.Instance.SFXVolume = sFXVolume;
    }
}
