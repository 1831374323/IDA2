using JetBrains.Annotations;
using SY_Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTest : MonoBehaviour
{

    //��ʼ��ʧ��λ�ã����³��ֵ�λ�ã������������ٽ�㣩
    public Vector2 startPos, endPos;

    //�ƶ���valueֵ
    private float value;

    //ÿ���������λ�����꣨����Ϊ������������
    Vector2[] vc = new Vector2[7];
    int[] CurrentId=new int[7];
    int[] IDs;//��¼����id
    //�Ƿ�ʼ�ƶ�
    bool Move;

    DataTable CardDataTable;
    void Start()
    {
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");//��ȡdatatable
        IDs=new int[CardDataTable.Rows.Count];
        for(int i = 0; i < CardDataTable.Rows.Count; i++)//��ֵ����id����
        {
            IDs[i] = Convert.ToInt32(CardDataTable.Rows[i][0]);
        }
        for(int i = 0; i < IDs.Length; i++)
        {
            if(IDs[i] == HandbookStatus.Instance.id)
            {
                int cur = (i - 3+IDs.Length)%IDs.Length;
                for(int j = 0; j < 7; j++)//�������ϸ��ϳ�ֵ
                {
                    transform.GetChild(j).name=cur.ToString();
                    if (Resources.Load<Sprite>("CardIcon/" + IDs[cur]) == null)
                    {
                        transform.GetChild(j).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
                        CurrentId[j] = cur;
                    }
                    else
                    {
                        transform.GetChild(j).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + IDs[cur] );
                        CurrentId[j] = cur;
                    }       
                    cur=(cur+1)%IDs.Length;
                }
                break;
            }
        }


        InitBtnPosition();
        transform.GetChild((transform.childCount - 1) / 2).localScale = 1.2f * Vector2.one;//���м�һ���Ŵ�
        startPos.y=transform.GetChild(0).localPosition.y;
        endPos.y=transform.GetChild(6).localPosition.y;
    }

    public void ButtonUseMoveDown()
    {
            //ȷ���ϴ��ƶ����
            if (value == 0)
            {

                for (int i = 0; i < transform.childCount; i++)
                {
                    //��¼�����е�ǰ���������ʼλ��
                    vc[i] = transform.GetChild(i).localPosition;
                }
                Move = true;
                StartCoroutine(MoveD());//����Э��
            }
    }

    public void ButtonUseMoveUp()
    {
        if (value == 0)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                vc[i] = transform.GetChild(i).localPosition;

            }
            Move = true;
            StartCoroutine(MoveU());
        }
    }


    void Update()
    {
        //���¼����¼�ͷ�������ƶ���
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {  
            ButtonUseMoveDown();
        }

       
        //���¼����Ҽ�ͷ�������ƶ���
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            ButtonUseMoveUp(); 
        }
    }
    /// <summary>
    /// �����ƶ�
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveD()
    {
        while (Move)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < transform.childCount; i++)
            {
                //�ƶ����(value > 1)�󣬳�ʼ��value����ʼ�ж�
                if (value > 1)
                {
                    value = 0;
                    //���һ�������峬���ױ��ٽ��
                    if (transform.GetChild(transform.childCount - 1).localPosition.y <= endPos.y)
                    {
                        //�ص����ұ�
                        transform.GetChild(transform.childCount - 1).localPosition = new Vector2(transform.GetChild(0).localPosition.x, startPos.y);
                        //��������������е����һ��
                        transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
                    }
                    Move = false;//�ر�ѭ��
                    MoveDownChange();
                    break;//�ڴ˴��жϣ�������벻��ִ��
                }
                //�ƶ����̣����Լ������ֲ�������
                value += Time.deltaTime * 0.5f;
                float moveheight = transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
                transform.GetChild(i).localPosition = Vector2.Lerp(vc[i], new Vector2(0, vc[i].y - moveheight), value);

                //�����ı���СͼƬ���м������任��С������ʵ�������
                //transform.GetChild(0).localScale = 0.8f * Vector2.one;
                transform.GetChild(2).localScale = Vector2.Lerp(Vector2.one, 1.2f * Vector2.one, value);
                transform.GetChild(3).localScale = Vector2.Lerp(1.2f * Vector2.one, Vector2.one, value);
                //transform.GetChild(3).localScale = 0.8f * Vector2.one;
            }
        }
    }
    /// <summary>
    /// �����ƶ�
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveU()
    {
        while (Move)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (value > 1)
                {
                    value = 0;
                    if (transform.GetChild(0).localPosition.x >= startPos.x)
                    {
                        //�ص������
                        transform.GetChild(0).localPosition = new Vector2(transform.GetChild(3).localPosition.x, endPos.y);
                        //��������������еĵ�һ��
                        transform.GetChild(0).SetAsLastSibling();
                        //��ʾ��ǰ����
                        //ToggleGroup.GetChild(0).SetAsLastSibling();
                    }
                    Move = false;
                    MoveUpChange();
                    break;
                }

                value += Time.deltaTime * 0.5f;
                //new Vector2(vc[i].x + 1440, 0)���ڵ�ǰ��ʼλ���������ƶ�100
                float moveheight = transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
                transform.GetChild(i).localPosition = Vector2.Lerp(vc[i], new Vector2(0, vc[i].y +moveheight ), value);

                transform.GetChild(3).localScale = Vector2.Lerp(1.2f * Vector2.one, Vector2.one, value);
                transform.GetChild(4).localScale = Vector2.Lerp(Vector2.one, 1.2f * Vector2.one, value);
            }
        }
    }

    void InitBtnPosition()
    {
        float startY = transform.GetChild(0).GetComponent<RectTransform>().rect.height * 6;//���Ϊʲô��getchild��0��
        float endY = -startY;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = new Vector2(0,startY);
            startY -= transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
        }
    }

    void MoveDownChange()//������down��ʱ������ͼƬ�����ĸı�
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            CurrentId[i] = (CurrentId[i]-1+IDs.Length)%IDs.Length;
            transform.GetChild(i).name = CurrentId[i].ToString();
            if (Resources.Load<Sprite>("CardIcon/" + IDs[CurrentId[i]]) == null)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
            }
            else
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + IDs[CurrentId[i]]);
            }
        }
        HandbookStatus.Instance.ChangeData(IDs[CurrentId[(transform.childCount - 1) / 2]]);
    }

    void MoveUpChange()//������up��ʱ������ͼƬ�����ĸı�,�������Э��������
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CurrentId[i] = (CurrentId[i] + 1 + IDs.Length) % IDs.Length;
            transform.GetChild(i).name = CurrentId[i].ToString();
            if (Resources.Load<Sprite>("CardIcon/" + IDs[CurrentId[i]]) == null)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
            }
            else
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + IDs[CurrentId[i]]);
            }
        }
        HandbookStatus.Instance.ChangeData(IDs[CurrentId[(transform.childCount - 1) / 2]]);
    }
}
