using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using SY_Tool;
using System;

public class ButtonsInit : MonoBehaviour
{
    public int num=1;//总数量
    public int _id;
    public GameObject Detailed;
    private DataTable CardDataTable;
    [SerializeField] int ifEnemyContainer;
    

    void Start()
    {  
        GameObject Btn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Perfabs/Handbook/Button.prefab");//找到方格资源
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");//获取怪物数据
        num= CardDataTable.Rows.Count;
        Debug.Log(num);
        Detailed.SetActive(false);//初始化界面
        for (int i = 0; i < num; i++)//初始化按钮
        {
            if (Convert.ToInt32(CardDataTable.Rows[i][13]) == ifEnemyContainer)//判断是否是敌我
            {
            GameObject btn = Instantiate(Btn);
            btn.name = Convert.ToString(CardDataTable.Rows[i][0]);//将按钮名字改成id
            btn.transform.SetParent(transform, false);//设置父物体
            if(Resources.Load<Sprite>("CardIcon/" + Convert.ToString(CardDataTable.Rows[i][0])) == null)//在设置之前判段是否存在
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);

            }
            else
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + Convert.ToString(CardDataTable.Rows[i][0]));
            }
                int x = Convert.ToInt32(CardDataTable.Rows[i][0]);
                btn.GetComponent<Button>().onClick.AddListener(OpenDetailedUI);
                btn.GetComponent<Button>().onClick.AddListener(() => { IdTransmit(x); });//事件传参
                    if (ifEnemyContainer == 1)
                    {
                        btn.GetComponent<Button>().onClick.AddListener(ChangeStateEnemy);
                    }
                    else
                    {
                        btn.GetComponent<Button>().onClick.AddListener(ChangeStateFriend);
                    }
                }
        }
        
    }

    void OpenDetailedUI()
    {
        Detailed.SetActive(true);
    }
    void IdTransmit(int a)
    {
        HandbookStatus.Instance.ChangeData(a);
    }

    private void ChangeStateEnemy()
    {
        HandbookStatus.Instance.state=HandbookStatus.State.enemy;
    }
    private void ChangeStateFriend()
    {
        HandbookStatus.Instance.state = HandbookStatus.State.friend;
    }
}
