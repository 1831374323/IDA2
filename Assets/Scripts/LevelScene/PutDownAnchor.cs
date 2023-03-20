using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutDownAnchor : MonoBehaviour
{
    void Start()
    {
        Invisible();
    }

    void Update()
    {
        CheckState();
    }

    //检查鼠标是否选中并更新自己的可见状态
    private void CheckState()
    {

        if (MouseManager.Instance.IsHolding)
        {
            Visiable();
        }
        else
        {
            Invisible();
        }
    }
    //隐藏自己
    private void Invisible()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
    }
    //显示自己
    private void Visiable()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
    }
}
