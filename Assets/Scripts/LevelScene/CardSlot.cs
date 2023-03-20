using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Data;
using TMPro;
using UnityEngine.UI;
public class CardSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    private int id;
    GameObject cardDetail;
    DataRow cardData;
    private float cardCD;
    Image cdMask;
    private void Awake()
    {
        cardDetail = transform.Find("CardDetail").gameObject;

        cdMask = transform.Find("cdMask").GetComponent<Image>();
        cdMask.raycastTarget = false;

    }
    public void Init(int _id)
    {
        //加载立绘
        if (Resources.Load<Sprite>("CardIcon/" + _id) != null)
        {
            transform.Find("CardIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + _id);
            cardDetail.transform.Find("CardIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + _id);
        }
        else
        {
            transform.Find("CardIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 00000);
            cardDetail.transform.Find("CardIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("CardIcon/" + 10001);
        }

        id = _id;

        //反射 挂载slot脚本
        string scriptName;
        scriptName = "Slot_" + id.ToString();

        Type _t = Type.GetType(scriptName);

        if (_t != null)
        {
            gameObject.AddComponent(_t);
        }
        else
        {
            gameObject.AddComponent<EntitySlot>();
        }


        cardData = EntitySpawner.Instance.cardDataTable.Rows.Find(id.ToString());       //获取生物信息
        cardCD = Convert.ToSingle(cardData["cardCD"]);
        gameObject.name = "CardSlot_" + id.ToString();
        InitDetail();                   //初始化卡牌详细信息
    }

    private void FixedUpdate()
    {
        if (cdTimer > 0)
        {
            cdTimer -= Time.deltaTime;
            cdMask.fillAmount = cdTimer / cardCD;
        }
    }

    #region ---鼠标事件---

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!FightManager.Instance.isPaused && cdTimer < 0.01f)
        {
            MouseManager.Instance.SetCurrentHoldEntity(id);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!FightManager.Instance.isPaused)
        {
            ShowDetail();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!FightManager.Instance.isPaused)
        {
            HideDetail();
        }
    }

    #endregion

    #region ----信息展示----
    private void ShowDetail()
    {
        cardDetail.SetActive(true);
    }

    private void HideDetail()
    {
        cardDetail.SetActive(false);
    }

    private void InitDetail()
    {
        SetMessage("speed", cardDetail.transform);
        SetMessage("atk", cardDetail.transform);
        SetMessage("hp", cardDetail.transform);
        SetMessage("cardCD", cardDetail.transform);
        SetMessage("cost", transform);
        SetMessage("name", transform);
    }

    /// <summary>
    /// 设置数值显示
    /// </summary>
    /// <param name="str">要设置的物体名=CSV中的列名</param>
    private void SetMessage(string str, Transform father)
    {
        string data;
        Transform _text = father.Find(str);
        data = cardData[str].ToString();
        _text.GetComponent<TMP_Text>().text = data;
        _text.GetComponent<TMP_Text>().raycastTarget = false;
        if (_text.Find("Icon") != null)
        {
            _text.Find("Icon").GetComponent<Image>().raycastTarget = false;
        }
    }

    #endregion

    [SerializeField] private float cdTimer = 0;
    public float CdTimer { get { return cdTimer; } }
    public void EnterCD()
    {
        cdTimer = cardCD;
    }

}
