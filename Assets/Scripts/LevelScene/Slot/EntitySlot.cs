using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySlot : MonoBehaviour
{
    protected int cardCD;
    void Start()
    {
        gameObject.GetComponent<CardSlot>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
