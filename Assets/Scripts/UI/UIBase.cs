using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    //显示界面
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    //隐藏界面
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    //关闭界面（销毁）
    public virtual void Close()
    {
        UIManager.Instance.CloseUI(gameObject.name);
    }
}
