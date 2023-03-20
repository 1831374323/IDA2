using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using UnityEngine.UI;
public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    [SerializeField] private int currentHoldEntityID = -1;      //当前选中生物ID
    private Vector3 mousePosition;               //当前鼠标位置
    private bool isHoldingEntity = false;        //当前是否选中生物
    public bool IsHolding { get { return isHoldingEntity; } }       //只读
    private GameObject holdEntity;      // 当前选中生物图标

    private void Awake()
    {
        Instance = this;
        HoldEntityInit();       //初始化选中生物图标
    }


    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);                //更新鼠标位置


        if (isHoldingEntity && !FightManager.Instance.isPaused)
        {
            HoldingUpdate();        //更新选中生物状态
        }

    }

    void FixedUpdate()
    {

    }

    #region ----选中生物图标控制----
    //初始化选中生物图标
    private void HoldEntityInit()
    {
        holdEntity = new GameObject("holdEntity");
        holdEntity.transform.parent = GameObject.Find("LevelCanvas").transform;
        holdEntity.AddComponent<Image>();
        holdEntity.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);                        //更改透明度
        holdEntity.GetComponent<Image>().raycastTarget = false;
        holdEntity.transform.localScale = new Vector3(-3 * holdEntity.transform.localScale.x, 3 * holdEntity.transform.localScale.y, 3 * holdEntity.transform.localScale.z);

        holdEntity.SetActive(false);
    }

    //激活选中生物图标
    public void SetCurrentHoldEntity(int id)
    {
        currentHoldEntityID = id;
        holdEntity.SetActive(true);
        holdEntity.GetComponent<Image>().sprite = EntitySpawner.Instance.GetEntitySprite(id);           //更改图片
        holdEntity.GetComponent<Image>().SetNativeSize();

        isHoldingEntity = true;
    }

    //取消选中生物图标
    public void ReleaseHoldEntity()
    {
        isHoldingEntity = false;
        holdEntity.SetActive(false);
        currentHoldEntityID = -1;
    }
    #endregion

    private void HoldingUpdate()
    {
        //holdEntity.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);   //更新图标位置

        holdEntity.transform.position = Input.mousePosition;        //更新图标位置
        //取消选中
        if (Input.GetMouseButtonDown(1))
        {

            ReleaseHoldEntity();
        }

        #region ----检测锚点、放置生物----
        GameObject anchor;
        anchor = MouseRay("LevelUI");

        //排空
        if (anchor != null)
        {
            //检测锚点
            if (anchor.tag == "PutAnchor")
            {
                //进入锚点范围的效果
                anchor.GetComponent<SpriteRenderer>().color = Color.red;

                //放置生物
                if (Input.GetMouseButtonDown(0))
                {
                    //根据id，从表中获取该生物的费用
                    DataRow dr = EntitySpawner.Instance.cardDataTable.Rows.Find(currentHoldEntityID.ToString());
                    int cost = Convert.ToInt32(dr["cost"]);
                    if (FightManager.Instance.myCost >= cost)
                    {
                        Entity entity = EntitySpawner.Instance.Spwan(currentHoldEntityID, anchor.transform.position, anchor.transform.parent);
                        FightManager.Instance.myCost -= entity.Cost;        //扣除费用
                        GameObject slot = GameObject.Find("CardSlot_" + currentHoldEntityID.ToString());        //重置CD
                        slot.GetComponent<CardSlot>().EnterCD();
                        ReleaseHoldEntity();
                    }
                }
            }
        }
        #endregion

    }

    private GameObject MouseRay()
    {
        RaycastHit2D hit;
        GameObject obj = null;         //射线检测到的物体

        hit = Physics2D.Raycast(new Vector2(mousePosition.x, mousePosition.y), Vector2.zero);
        if (hit.collider != null)
        {
            obj = hit.collider.gameObject;
        }
        return obj;
    }
    private GameObject MouseRay(string layer)
    {

        GameObject obj = null;         //射线检测到的物体
        LayerMask _layerMask = 1 << LayerMask.NameToLayer(layer);

        //RaycastHit2D hit;
        //hit = Physics2D.Raycast(new Vector2(mousePosition.x, mousePosition.y), Vector2.zero, 1, _layerMask);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 10f, _layerMask);
        if (hit.collider != null)
        {
            obj = hit.collider.gameObject;
        }

        return obj;
    }
}
