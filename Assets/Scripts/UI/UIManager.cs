using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private Transform canvasTf;
    private List<UIBase> uiList;    //存储加载过的界面

    private void Awake()
    {
        Instance = this;
        canvasTf = GameObject.Find("").transform;
        uiList = new List<UIBase>();
    }

    //显示界面
    public UIBase ShowUI<T>(string uiName) where T : UIBase
    {
        UIBase ui = Find(uiName);
        if (ui = null)
        {
            //集合中没有则从Resources加载
            GameObject obj = Instantiate(Resources.Load("UI/" + uiName), canvasTf) as GameObject;

            //重命名
            obj.name = uiName;

            //添加脚本
            ui = obj.AddComponent<T>();

            //添加到集合
            uiList.Add(ui);
        }
        else
        {
            ui.Show();
        }

        return ui;
    }

    //隐藏界面
    public void HideUI(string uiName)
    {
        UIBase ui = Find(uiName);
        if (ui != null)
        {
            ui.Hide();
        }
    }

    //关闭所有界面
    public void CloseAllUI()
    {
        foreach (var ui in uiList)
        {
            ui.Close();
        }
    }

    //关闭某个界面
    public void CloseUI(string uiName)
    {
        UIBase ui = Find(uiName);
        if (ui != null)
        {
            uiList.Remove(ui);
            Destroy(ui.gameObject);
        }
    }
    
    //从list中找到对应界面
    public UIBase Find(string uiName)
    {
        foreach (var ui in uiList)
        {
            if (ui.name == uiName)
            {
                return ui;
            }
        }
        return null;
    }
}
