using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;
using System.IO;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    struct SpwanData
    {
        public string id;
        public string mode;
        public float time;
    }
    List<SpwanData> unLoop_CostBase = new List<SpwanData>();
    List<SpwanData> Loop_CostBase = new List<SpwanData>();



    #region  ----生命周期----

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Init();
    }

    void Init()
    {
        ReadXml(FightManager.Instance.level_ID);
    }

    void Start()
    {
        StartCoroutine(CostAutoIncrease());
    }
    public float m_time;
    private void FixedUpdate()
    {
        m_time += Time.deltaTime;
        m_FixUpdate();
    }

    void m_FixUpdate()
    {
        EnemyProduce();
    }

    #endregion

    #region ----路径选择----
    [Serializable]
    public class Road : IComparable<Road>
    {
        [HideInInspector]
        public int combat;
        public GameObject roadPoint;
        public int CompareTo(Road other)
        {
            if (other == null) return 1;
            return combat.CompareTo(other.combat);
        }
    }
    public List<Road> roads = new List<Road>();

    /// <summary>
    /// 根据召唤模式获取对应路径(暂时用费用代替战力)
    /// </summary>
    /// <returns>roadpoint</returns>
    GameObject GetRoad(string mode)
    {
        //设置每条路的战力
        for (var i = 0; i < roads.Count; i++)
        {
            roads[i].combat = 0;
            //相加路上每个生物的费用，设置为战力
            foreach (var entity in roads[i].roadPoint.transform.GetComponentsInChildren<Entity>())
            {
                roads[i].combat += entity.Cost;
            }
        }
        //根据战力排序
        List<Road> temp = new List<Road>();
        roads.ForEach(i => temp.Add(i));
        temp.Sort();

        //根据模式返回路径
        switch (mode)
        {
            case "strong":
                return temp[roads.Count - 1].roadPoint;
            case "week":
                return temp[0].roadPoint;
            default:
                return temp[0].roadPoint;
        }
    }
    #endregion
    int loopNum = 0;        //循环标记

    /// <summary>
    /// 生成敌人
    /// </summary>
    void EnemyProduce()
    {
        //非循环状态
        if (unLoop_CostBase.Count > 0)
        {
            //按费用召唤
            //if (myCost >= GetCardCost(unLoop_CostBase[0].id))
            //按时间召唤
            if (m_time > unLoop_CostBase[0].time)
            {
                //根据召唤模式选择路线
                Transform target;
                switch (unLoop_CostBase[0].mode)
                {
                    case "strong":
                        target = GetRoad("strong").transform.Find("Target");
                        EntitySpawner.Instance.Spwan(Convert.ToInt32(unLoop_CostBase[0].id), target.position, target, true);
                        break;
                    case "week":
                        target = GetRoad("week").transform.Find("Target");
                        EntitySpawner.Instance.Spwan(Convert.ToInt32(unLoop_CostBase[0].id), target.position, target, true);
                        break;
                    default:
                        target = roads[Convert.ToInt32(unLoop_CostBase[0].mode)].roadPoint.transform.Find("Target");
                        EntitySpawner.Instance.Spwan(Convert.ToInt32(unLoop_CostBase[0].id), target.position, target, true);
                        break;
                }
                myCost -= GetCardCost(unLoop_CostBase[0].id);
                unLoop_CostBase.Remove(unLoop_CostBase[0]);
            }
            //进入循环模式，时间归零
            if (unLoop_CostBase.Count == 0)
            {
                m_time = 0;
            }
        }
        //循环状态
        else if (Loop_CostBase.Count > 0)
        {
            //if (myCost >= GetCardCost(Loop_CostBase[loopNum].id))
            if (m_time > Loop_CostBase[loopNum].time)
            {
                switch (Loop_CostBase[loopNum].mode)
                {
                    case "strong":
                        break;
                    case "week":
                        break;
                    default:
                        Transform target = roads[Convert.ToInt32(Loop_CostBase[loopNum].mode)].roadPoint.transform.Find("Target");
                        EntitySpawner.Instance.Spwan(Convert.ToInt32(Loop_CostBase[loopNum].id), target.position, target, true);
                        break;
                }
                myCost -= GetCardCost(Loop_CostBase[loopNum].id);
                if (loopNum < Loop_CostBase.Count - 1)          //还在循环中
                {
                    loopNum++;
                }
                else                                           //本次循环结束
                {
                    loopNum = 0;        //重置flag
                    m_time = 0;           //重置时间
                }
            }
        }
    }

    /// <summary>
    /// 根据id获取卡牌费用
    /// </summary>
    /// <returns>卡牌费用</returns>
    private int GetCardCost(string id)
    {
        int cost;
        cost = Convert.ToInt32(EntitySpawner.Instance.GetCardData(id, "cost"));
        return cost;
    }

    #region  ----敌人(模拟)费用管理----
    float getCostValue = 1;
    float getCostInterval = 2;
    [SerializeField]
    float myCost = 0;

    /// <summary>
    /// 敌人费用管理
    /// </summary>
    IEnumerator CostAutoIncrease()
    {
        while (true)
        {
            myCost += getCostValue;
            yield return new WaitForSeconds(getCostInterval);
        }
    }

    #endregion
    void ReadXml(string _levelID)                               //将LevelEnemy数据载入
    {

        //加载xml文件
        XmlDocument xml = new XmlDocument();
        //TextAsset xmlFile = Resources.Load<TextAsset>("LevelEnemy");
        //xml.LoadXml(xmlFile.text);


        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;

        FileStream fs = new FileStream(Application.streamingAssetsPath + "/LevelEnemy.xml", FileMode.OpenOrCreate, FileAccess.Read);
        XmlReader reader = XmlReader.Create(fs, settings);
        xml.Load(reader);


        XmlNodeList levelNodes = xml.SelectSingleNode("root").ChildNodes;     //一级节点 关卡节点

        foreach (XmlElement levelNode in levelNodes)
        {
            if (levelNode.GetAttribute("id") == _levelID)                    //进入关卡节点
            {
                foreach (XmlElement levelData in levelNode.ChildNodes)
                {

                    //if (_levelData.GetType() == typeof(XmlElement))
                    //{
                    //XmlElement levelData = (XmlElement)_levelData;
                    if (levelData.Name == ("getCostValue"))
                    {
                        getCostValue = Convert.ToSingle(levelData.InnerText);
                    }
                    else if (levelData.Name == "getCostInterval")
                    {
                        getCostInterval = Convert.ToSingle(levelData.InnerText);
                    }
                    else if (levelData.Name == ("unLoop"))                                        //处理非循环模式
                    {
                        foreach (XmlElement spwanMode in levelData.ChildNodes)
                        {
                            if (spwanMode.Name == ("costBase"))
                            {                               //基于费用的召唤
                                foreach (XmlElement spwanData in spwanMode.ChildNodes)
                                {
                                    SpwanData data = new SpwanData();
                                    data.id = spwanData.GetAttribute("id");
                                    data.time = Convert.ToSingle(spwanData.GetAttribute("time"));
                                    data.mode = spwanData.InnerText;
                                    unLoop_CostBase.Add(data);
                                }
                            }
                        }
                    }
                    else if (levelData.Name == ("loop"))       //处理循环模式
                    {
                        foreach (XmlElement spwanMode in levelData.ChildNodes)
                        {
                            if (spwanMode.Name == ("costBase"))
                            {                               //基于费用的召唤
                                foreach (XmlElement spwanData in spwanMode.ChildNodes)
                                {
                                    SpwanData data = new SpwanData();
                                    data.id = spwanData.GetAttribute("id");
                                    data.time = Convert.ToSingle(spwanData.GetAttribute("time"));
                                    data.mode = spwanData.InnerText;
                                    Loop_CostBase.Add(data);
                                }
                            }
                        }
                    }
                    //}
                }
            }
        }
    }
}
