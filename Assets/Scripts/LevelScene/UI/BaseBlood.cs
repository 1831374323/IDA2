using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseBlood : MonoBehaviour
{
    public GameObject baseObj;
    private Basement basement;
    void Start()
    {
        basement = baseObj.GetComponent<Basement>();
    }

    private void FixedUpdate()
    {
        GetComponent<Image>().fillAmount = basement.CurHp / basement.maxHp;
    }
}
