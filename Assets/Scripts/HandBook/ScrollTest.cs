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

    //开始消失的位置，重新出现的位置（即左右两端临界点）
    public Vector2 startPos, endPos;

    //移动的value值
    private float value;

    //每个子物体的位置坐标（长度为子物体总量）
    Vector2[] vc = new Vector2[7];
    int[] CurrentId=new int[7];
    int[] IDs;//记录总体id
    //是否开始移动
    bool Move;

    DataTable CardDataTable;
    void Start()
    {
        CardDataTable = SY_CSV.ReadFromResources("Data/CardData");//获取datatable
        IDs=new int[CardDataTable.Rows.Count];
        for(int i = 0; i < CardDataTable.Rows.Count; i++)//把值赋给id数组
        {
            IDs[i] = Convert.ToInt32(CardDataTable.Rows[i][0]);
        }
        for(int i = 0; i < IDs.Length; i++)
        {
            if(IDs[i] == HandbookStatus.Instance.id)
            {
                int cur = (i - 3+IDs.Length)%IDs.Length;
                for(int j = 0; j < 7; j++)//给界面上赋上初值
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
        transform.GetChild((transform.childCount - 1) / 2).localScale = 1.2f * Vector2.one;//把中间一个放大
        startPos.y=transform.GetChild(0).localPosition.y;
        endPos.y=transform.GetChild(6).localPosition.y;
    }
    void Update()
    {
        //按下键盘左箭头（向下移动）
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //确保上次移动完成
            if (value == 0)
            {
                
                for (int i = 0; i < transform.childCount; i++)
                {
                    //记录下所有当前子物体的起始位置
                    vc[i] = transform.GetChild(i).localPosition;
                }
                Move = true;
                StartCoroutine(MoveD());//开启协程
            }
            

        }
        //按下键盘右箭头（向上移动）
        if (Input.GetKeyDown(KeyCode.UpArrow))
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
    }
    /// <summary>
    /// 向下移动
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveD()
    {
        while (Move)
        {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < transform.childCount; i++)
            {
                //移动完成(value > 1)后，初始化value，开始判断
                if (value > 1)
                {
                    value = 0;
                    //最后一个子物体超过底边临界点
                    if (transform.GetChild(transform.childCount - 1).localPosition.y <= endPos.y)
                    {
                        //回到最右边
                        transform.GetChild(transform.childCount - 1).localPosition = new Vector2(transform.GetChild(0).localPosition.x, startPos.y);
                        //把它变成子物体中的最后一个
                        transform.GetChild(transform.childCount - 1).SetAsFirstSibling();
                    }
                    Move = false;//关闭循环
                    MoveDownChange();
                    break;//在此处中断，下面代码不再执行
                }
                //移动进程（可自己调整轮播快慢）
                value += Time.deltaTime * 0.5f;
                float moveheight = transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
                transform.GetChild(i).localPosition = Vector2.Lerp(vc[i], new Vector2(0, vc[i].y - moveheight), value);

                //其他的保持小图片，中间两个变换大小（根据实际情况）
                //transform.GetChild(0).localScale = 0.8f * Vector2.one;
                transform.GetChild(2).localScale = Vector2.Lerp(Vector2.one, 1.2f * Vector2.one, value);
                transform.GetChild(3).localScale = Vector2.Lerp(1.2f * Vector2.one, Vector2.one, value);
                //transform.GetChild(3).localScale = 0.8f * Vector2.one;
            }
        }
    }
    /// <summary>
    /// 向上移动
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
                        //回到最左边
                        transform.GetChild(0).localPosition = new Vector2(transform.GetChild(3).localPosition.x, endPos.y);
                        //把它变成子物体中的第一个
                        transform.GetChild(0).SetAsLastSibling();
                        //显示当前进度
                        //ToggleGroup.GetChild(0).SetAsLastSibling();
                    }
                    Move = false;
                    MoveUpChange();
                    break;
                }

                value += Time.deltaTime * 0.5f;
                //new Vector2(vc[i].x + 1440, 0)：在当前初始位置上向上移动100
                float moveheight = transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
                transform.GetChild(i).localPosition = Vector2.Lerp(vc[i], new Vector2(0, vc[i].y +moveheight ), value);

                transform.GetChild(3).localScale = Vector2.Lerp(1.2f * Vector2.one, Vector2.one, value);
                transform.GetChild(4).localScale = Vector2.Lerp(Vector2.one, 1.2f * Vector2.one, value);
            }
        }
    }

    void InitBtnPosition()
    {
        float startY = transform.GetChild(0).GetComponent<RectTransform>().rect.height * 6;//这边为什么是getchild（0）
        float endY = -startY;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = new Vector2(0,startY);
            startY -= transform.GetChild(0).GetComponent<RectTransform>().rect.height*2;
        }
    }

    void MoveDownChange()//当按下down键时，滚动图片发生的改变
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

    void MoveUpChange()//当按下up键时，滚动图片发生的改变,必须得在协程里引用
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
