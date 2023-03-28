using SY_Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTest : MonoBehaviour
{

    //��ʼ��ʧ��λ�ã����³��ֵ�λ�ã������������ٽ�㣩
    public Vector2 startPos, endPos;
    [SerializeField] private int intervalValue=70;
    //�ƶ���valueֵ
    private float value;

    //ÿ���������λ�����꣨����Ϊ������������
    private Vector2[] vc = new Vector2[7];
    private int[] CurrentId = new int[7];
    private int[] IDs;//��¼����id
    private List<int> friendIDs;
    private List<int> enemyIDs;

    //�Ƿ�ʼ�ƶ�
    bool Move;

    DataTable CardDataTable;
    void Start()
    {
        
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");//��ȡdatatable
        GetAllIDs();
        #region ��ʼ������
        if (HandbookStatus.Instance.state == HandbookStatus.State.friend)
        {
            DataInit(friendIDs);
        }
        else
        {
            DataInit(enemyIDs);
        }
        

        #endregion


        InitBtnPosition();
        transform.GetChild((transform.childCount - 1) / 2).localScale = 1.2f * Vector2.one;//���м�һ���Ŵ�
        startPos.y = transform.GetChild(0).localPosition.y;
        endPos.y = transform.GetChild(6).localPosition.y;

        AddButtonEvent();
    }
    void Update()
    {
        //���¼����ϼ�ͷ�������ƶ���
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //ȷ���ϴ��ƶ����
            if (value == 0)
            {

               UpdateChildPosition();
                Move = true;
                StartCoroutine(MoveD(1));//����Э��
            }


        }
        //���¼����Ҽ�ͷ�������ƶ���
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (value == 0)
            {

                UpdateChildPosition();
                Move = true;
                StartCoroutine(MoveU(1));
            }

        }
    }
    /// <summary>
    /// �����ƶ�
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveD(int count)
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
                    count--;
                    if (count == 0)
                    {
                        Move = false;
                        UpdateChildPosition();
                        if (HandbookStatus.Instance.state == HandbookStatus.State.friend) MoveDownChange(friendIDs);
                        else  MoveDownChange(enemyIDs);
                        break;//�ڴ˴��жϣ�������벻��ִ��
                    }//�ر�ѭ��
                    else
                    {
                        UpdateChildPosition();
                        UpdateChildPosition();
                        if (HandbookStatus.Instance.state == HandbookStatus.State.friend) MoveDownChange(friendIDs);
                        else MoveDownChange(enemyIDs);
                    }
                        
                    }
                    //�ƶ����̣����Լ������ֲ�������
                    value += Time.deltaTime * 0.5f;
                    float moveheight = (transform.GetChild(0).GetComponent<RectTransform>().rect.height + intervalValue);
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
    IEnumerator MoveU(int count)
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
                        }
                    count--;
                    if (count == 0)
                    {
                        Move = false;
                        if(HandbookStatus.Instance.state == HandbookStatus.State.friend) MoveUpChange(friendIDs);
                        else MoveUpChange(enemyIDs);
                        break;
                    }//�ر�ѭ��
                    else
                    {
                        UpdateChildPosition();
                        if (HandbookStatus.Instance.state == HandbookStatus.State.friend) MoveUpChange(friendIDs);
                        else MoveUpChange(enemyIDs);
                    }
                    
                    }

                    value += Time.deltaTime * 0.5f;
                    //new Vector2(vc[i].x + 1440, 0)���ڵ�ǰ��ʼλ���������ƶ�100
                    float moveheight = (transform.GetChild(0).GetComponent<RectTransform>().rect.height + intervalValue);
                    transform.GetChild(i).localPosition = Vector2.Lerp(vc[i], new Vector2(0, vc[i].y + moveheight), value);

                    transform.GetChild(3).localScale = Vector2.Lerp(1.2f * Vector2.one, Vector2.one, value);
                    transform.GetChild(4).localScale = Vector2.Lerp(Vector2.one, 1.2f * Vector2.one, value);
                }
            }
    }

    void InitBtnPosition()
    {
        float startY = transform.GetChild(0).GetComponent<RectTransform>().rect.height * 3 + intervalValue * 3;//���Ϊʲô��getchild��0��
        float endY = -startY;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = new Vector2(0, startY);
            startY -= transform.GetChild(0).GetComponent<RectTransform>().rect.height + intervalValue;
        }
    }

    void MoveDownChange(List<int> list)//������down��ʱ������ͼƬ�����ĸı�
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CurrentId[i] = (CurrentId[i] - 1 + list.Count) % list.Count;
            transform.GetChild(i).name = CurrentId[i].ToString();
            if (Resources.Load<Sprite>("CardIcon/" + list[CurrentId[i]]) == null)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
            }
            else
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + list[CurrentId[i]]);
            }
        }

        AddButtonEvent();
        HandbookStatus.Instance.ChangeData(list[CurrentId[(transform.childCount - 1) / 2]]);
    }

    void MoveUpChange(List<int> list)//������up��ʱ������ͼƬ�����ĸı�,�������Э��������
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CurrentId[i] = (CurrentId[i] + 1 + list.Count) % list.Count;
            transform.GetChild(i).name = CurrentId[i].ToString();
            if (Resources.Load<Sprite>("CardIcon/" + list[CurrentId[i]]) == null)
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
            }
            else
            {
                transform.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + list[CurrentId[i]]);
            }
        }

        AddButtonEvent();
        HandbookStatus.Instance.ChangeData(list[CurrentId[(transform.childCount - 1) / 2]]);
    }

    private void AddButtonEvent()
    {
        for(int i=0;i< transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.GetComponent<Button>().onClick.RemoveAllListeners();
            int x = i;
            child.GetComponent<Button>().onClick.AddListener(() => { AddChangeToCentre(x); });
            
        }
    }

    private void AddChangeToCentre(int a)
    {
        if (a < 3)
        {
            if (value == 0)
            {

                for (int i = 0; i < transform.childCount; i++)
                {
                    vc[i] = transform.GetChild(i).localPosition;

                }
                Move = true;
                StartCoroutine(MoveD(3-a));
                
            }
        }
        else if(a>3)
        { 
            if (value == 0)
            {

                for (int i = 0; i < transform.childCount; i++)
                {
                    vc[i] = transform.GetChild(i).localPosition;

                }
                Move = true;
                StartCoroutine(MoveU(a-3));
            }
        }
        else
        {

        }
    }

    public void ButtonUseMoveUp()
    {
        if (value == 0)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                //��¼�����е�ǰ���������ʼλ��
                vc[i] = transform.GetChild(i).localPosition;
            }
            Move = true;
            StartCoroutine(MoveD(1));//����Э��
        }
    }

    public void ButtonUseMoveDown()
    {
        if (value == 0)
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                vc[i] = transform.GetChild(i).localPosition;

            }
            Move = true;
            StartCoroutine(MoveU(1));
        }
    }

    private void UpdateChildPosition()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //��¼�����е�ǰ���������ʼλ��
            vc[i] = transform.GetChild(i).localPosition;
        }
    }

    private void GetAllIDs()
    {
        IDs = new int[CardDataTable.Rows.Count];
        enemyIDs = new List<int>();
        friendIDs = new List<int>();
        for (int i = 0; i < CardDataTable.Rows.Count; i++)//��ֵ����id����
        {
            IDs[i] = Convert.ToInt32(CardDataTable.Rows[i][0]);
            if (Convert.ToInt32(CardDataTable.Rows[i][13]) == 1)
            {
                enemyIDs.Add(IDs[i]);
            }
            else
            {
                friendIDs.Add(IDs[i]);
            }
        }
    }

    private bool ifNeedChange(List<int> idList)//�ж��Ƿ���Ҫ����ѭ��,��ʱ
    {
        if (idList.Count > 7) return true;
        else return false;
    }

    private void DataInit(List<int> IDList)
    {
        for (int i = 0; i < IDList.Count; i++)
        {
            if (IDList[i] == HandbookStatus.Instance.id)
            {
                int cur = (i - 3 + IDList.Count) % IDs.Length;
                for (int j = 0; j < 7; j++)//�������ϸ��ϳ�ֵ
                {
                    transform.GetChild(j).name = cur.ToString();
                    if (Resources.Load<Sprite>("CardIcon/" + IDList[cur]) == null)
                    {
                        transform.GetChild(j).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 0);
                        CurrentId[j] = cur;
                    }
                    else
                    {
                        transform.GetChild(j).GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + IDs[cur]);
                        CurrentId[j] = cur;
                    }
                    cur = (cur + 1) % IDList.Count;
                }
                break;
            }
        }
    }

    private void EnemyInit(List<int> IDList)
    {

    }
}
