using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    public string level_ID = "0";

    public int getCost = 1;
    public float getCostInterval = 2;

    GameObject escButton;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetBringCard();
        StartCoroutine(CostAutoIncrease());     //费用增长
        costUI = GameObject.Find("LevelCanvas/Cost");
        Time.timeScale = 1;
        escButton = GameObject.Find("ESC");
    }
    void Update()
    {
        CostUIUpdate();
    }

    #region ----卡组管理----
    private List<GameObject> CardSlots;
    public List<int> BringCardId;
    /// <summary>
    /// 设置本次战斗的卡组
    /// </summary>
    public void SetBringCard()
    {
        GameObject SlotTemplate = Resources.Load<GameObject>("CardSlot");
        Transform slots = GameObject.Find("CardSlots").transform;
        BringCardId.ForEach(
            i =>
            {
                GameObject slot = Instantiate(SlotTemplate, slots);
                slot.GetComponent<CardSlot>().Init(i);
            }
        );
        // int i = 10001;
        // foreach (var cardSlot in CardSlots)
        // {
        //     cardSlot.GetComponent<CardSlot>().Init(i);
        //     i++;
        // }

    }
    #endregion

    #region ----费用管理----
    public int myCost = 0;

    IEnumerator CostAutoIncrease()
    {
        while (true)
        {
            myCost += getCost;
            yield return new WaitForSeconds(getCostInterval);
        }
    }

    private GameObject costUI;
    private void CostUIUpdate()
    {
        costUI.GetComponent<TMP_Text>().text = "Cost:" + myCost.ToString();
    }
    #endregion

    #region ----游戏状态管理----

    public bool isPaused = false;
    public void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
    }
    public void UnPause()
    {
        Time.timeScale = 1;
        isPaused = false;
    }
    public void GameVectory()
    {
        Debug.Log("Game Vectory");
        escButton.GetComponent<ESCButton>().ShowEsc();
        escButton.SetActive(false);
        CardLock.UnLock(UnlockEntityID);
        LevelLock.UnLock(UnlockLevelID);
    }

    public void GameLoss()
    {
        Debug.Log("Game Loss");
        escButton.GetComponent<ESCButton>().ShowEsc();
        escButton.SetActive(false);
    }

    #endregion

    public string UnlockEntityID;

    public string UnlockLevelID;

}
