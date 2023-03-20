using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using SY_Tool;
using System.Data;
using Pathfinding;

public class EntitySpawner : MonoBehaviour
{

    public static EntitySpawner Instance;   //单例
    public GameObject entityTemplate;       //entity prefab
    public DataTable cardDataTable;         //存储卡牌信息的DataTable
    public Vector3 cameraAngle;
    private Dictionary<string, Sprite> spritesDictionary = new Dictionary<string, Sprite>();        // 存储单位模型的字典；<id，模型>
    private Dictionary<string, RuntimeAnimatorController> animDic = new Dictionary<string, RuntimeAnimatorController>();
    private void Awake()
    {
        Instance = this;
        EntitySpritesInit();            //将所有模型载入模型字典
        CardDataInit();                 //载入卡牌信息

    }

    /// <summary>
    /// 判断是否要将单位加入slot的观察列表
    /// </summary>
    public event Action<Entity> EvolveCheck;
    /// <summary>
    /// 生成单位
    /// </summary>
    /// <param name="parent">友方：RoadPoint 敌方：Target</param>

    public Entity Spwan(int id, Vector2 pos, Transform parent, bool isEnemy = false)
    {
        //生成生物模板
        GameObject entity = Instantiate(entityTemplate);
        //加载生物模型和脚本、设置单位数值
        EntityLoad(id, entity, isEnemy);
        //设置父物体
        entity.transform.parent = parent;
        //设置位置
        entity.transform.position = new Vector3(pos.x, pos.y, -0.75f);
        //设置寻路目标
        if (isEnemy)
        {
            entity.GetComponent<AIDestinationSetter>().target = parent.parent.Find("Anchor");
        }
        else
        {

            entity.GetComponent<AIDestinationSetter>().target = parent.Find("Target");
        }
        // 判断是否要将单位加入slot的观察列表
        // if (EvolveCheck != null)
        // {
        //     EvolveCheck(entity.GetComponent<Entity>());

        // }
        EvolveCheck?.Invoke(entity.GetComponent<Entity>());
        return entity.GetComponent<Entity>();
    }

    //加载模型、添加对应脚本、设置单位数值
    private void EntityLoad(int _id, GameObject entity, bool isEnemy)
    {
        string id = _id.ToString();
        //加载模型

        GameObject model = entity.transform.Find("model").gameObject;

        //model.GetComponent<SpriteRenderer>().sprite = GetEntitySprite(10001);

        //加载动作
        if (!animDic.ContainsKey(id))
        {
            animDic.Add(id, Resources.Load<RuntimeAnimatorController>(id + "/" + id));
        }

        if (animDic.ContainsKey(id) && animDic[id] != null)
        {
            model.GetComponent<Animator>().runtimeAnimatorController = animDic[id];
        }
        //反射
        string scriptName;
        scriptName = "Entity_" + id.ToString();         //获取类名

        Type _t = Type.GetType(scriptName);             //获取类

        if (_t != null)
        {
            entity.AddComponent(_t);                  //挂载脚本
        }
        else
        {
            entity.AddComponent<Entity_00000>();
        }

        //设置单位信息
        entity.name = scriptName;
        SetEntityData(_id, entity);
        entity.GetComponent<Entity>().isEnemy = isEnemy;

    }

    //private Sprite[] sprites;
    /// <summary>
    /// 将模型载入模型字典
    /// </summary>
    private void EntitySpritesInit()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");

        foreach (var sprite in sprites)
        {
            spritesDictionary.Add(sprite.name, sprite);
        }
    }

    //根据id获取Sprite
    public Sprite GetEntitySprite(int id)
    {
        if (spritesDictionary.ContainsKey(id.ToString()))
        {
            return spritesDictionary[id.ToString()];
        }
        else
        {
            return spritesDictionary["error"];
        }
    }

    //从CSV获取卡牌信息，载入dataTable
    private void CardDataInit()
    {
        cardDataTable = SY_CSV.ReadFromResources("Data/CardData");      //读取卡牌信息CSV
        //设置主键
        DataColumn[] keys = new DataColumn[1];
        keys[0] = cardDataTable.Columns["id"];
        cardDataTable.PrimaryKey = keys;


    }

    //从datatable设置单位数据
    private void SetEntityData(int id, GameObject entity)
    {
        if (entity.GetComponent<Entity>() != null)
        {
            //设置单位数据
            DataRow dr = cardDataTable.Rows.Find(id.ToString());
            //Debug.Log(dr["name"]);
            entity.GetComponent<Entity>().SetData(
                Convert.ToInt32(dr["id"]),
                Convert.ToString(dr["name"]),
                Convert.ToSingle(dr["atk"]),
                Convert.ToSingle(dr["speed"]),
                Convert.ToSingle(dr["attackInterval"]),
                Convert.ToSingle(dr["detectRange"]),
                Convert.ToSingle(dr["attackRange"]),
                Convert.ToSingle(dr["hp"]),
                Convert.ToInt32(dr["cost"]),
                Convert.ToInt32(dr["maxTarget"]),
                Convert.ToSingle(dr["combatValue"]),
                Convert.ToSingle(dr["cardCD"])
            );
        }
    }

    public string GetCardData(string id, string dataName)
    {
        DataRow dr = cardDataTable.Rows.Find(id);
        return dr[dataName].ToString();
    }
}
//id	name	cost	hp	speed	atk	attackRange	attackInterval	Detail	是否敌人	卡牌阶数	卡牌种类

