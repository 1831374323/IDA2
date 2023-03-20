using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCButton : MonoBehaviour
{
    GameObject escCanvas;
    private void Awake()
    {
        escCanvas = GameObject.Find("EscCanvas").transform.Find("bg").gameObject;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowEsc();
        }
    }
    public void OnClick()
    {
        ShowEsc();
    }

    public void ShowEsc()
    {
        if (!FightManager.Instance.isPaused)
        {
            FightManager.Instance.Pause();
            escCanvas.SetActive(true);
        }
        else
        {
            FightManager.Instance.UnPause();
            escCanvas.SetActive(false);
        }
    }
}
