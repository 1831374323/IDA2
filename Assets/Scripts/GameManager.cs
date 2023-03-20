using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {

    }

    #region ----Map_Level----
    public List<string> bringCard = new List<string>();     //带入关卡的生物列表


    /// <summary>
    /// 增加携带生物列表
    /// </summary>
    /// <returns>成功添加：true</returns>
    public bool AddBringCard(string id)
    {
        if (!bringCard.Contains(id))
        {
            bringCard.Add(id);
            return true;
        }
        return false;
    }
    #endregion
}
