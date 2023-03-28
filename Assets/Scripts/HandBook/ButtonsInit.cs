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
    public int num=1;//������
    public int _id;
    public GameObject Detailed;
    private DataTable CardDataTable;
    [SerializeField] int ifEnemyContainer;
    

    void Start()
    {  
        GameObject Btn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Perfabs/Handbook/Button.prefab");//�ҵ�������Դ
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");//��ȡ��������
        num= CardDataTable.Rows.Count;
        Debug.Log(num);
        Detailed.SetActive(false);//��ʼ������
        for (int i = 0; i < num; i++)//��ʼ����ť
        {
            if (Convert.ToInt32(CardDataTable.Rows[i][13]) == ifEnemyContainer)//�ж��Ƿ��ǵ���
            {
            GameObject btn = Instantiate(Btn);
            btn.name = Convert.ToString(CardDataTable.Rows[i][0]);//����ť���ָĳ�id
            btn.transform.SetParent(transform, false);//���ø�����
            if(Resources.Load<Sprite>("CardIcon/" + Convert.ToString(CardDataTable.Rows[i][0])) == null)//������֮ǰ�ж��Ƿ����
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);

            }
            else
            {
                btn.GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + Convert.ToString(CardDataTable.Rows[i][0]));
            }
                int x = Convert.ToInt32(CardDataTable.Rows[i][0]);
                btn.GetComponent<Button>().onClick.AddListener(OpenDetailedUI);
                btn.GetComponent<Button>().onClick.AddListener(() => { IdTransmit(x); });//�¼�����
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
