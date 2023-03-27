using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using SY_Tool;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;

public class HandbookStatus : MonoBehaviour
{
    public static HandbookStatus Instance { get; set; }
    // Start is called before the first frame update
    public int id = 0;
    DataTable CardDataTable;
    DataRow dr;
    void Awake()//��ʼ��
    {
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

       
    }
    private void DataInit()//��ʼ�����ݣ��ҵ���Ӧsprite
    {
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");
        DataColumn[] keys = new DataColumn[1];
        keys[0]=CardDataTable.Columns["id"];
        CardDataTable.PrimaryKey = keys;
        dr = CardDataTable.Rows.Find(id.ToString());
        transform.Find("Sprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + id);
    }
    private void SetProperty()//��������
    {
        Text Property = transform.Find("property").GetComponent<Text>();
        Property.text = "Cost:" + Convert.ToString(dr["cost"]) + "\n" +
                        "Hp:" + Convert.ToString(dr["hp"]) + "\n" +
                        "Speed:" + Convert.ToString(dr["speed"]) + "\n" +
                        "atk:" + Convert.ToString(dr["atk"]);
    }
    private void SetDescription()//���ý���
    {
        Text Description = transform.Find("Description").GetComponent<Text>();
        Description.text = Convert.ToString(dr["Detail"]);
    }

    public void ChangeData(int newId)
    {
        id = newId;
        DataInit();
        transform.Find("Name").GetComponent<Text>().text = Convert.ToString(dr["name"]);
        SetDescription();
        SetProperty();
    }
}
