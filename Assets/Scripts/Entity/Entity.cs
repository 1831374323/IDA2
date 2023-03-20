using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
public abstract class Entity : MonoBehaviour, IDoDamage
{

    #region ----单位数值----
    [SerializeField] protected int id = -1;
    [SerializeField] protected string entityName;           //单位名字
    [SerializeField] protected float atk = 1;               //攻击力
    [SerializeField] protected float speed = 0.2f;             //速度
    [SerializeField] protected float attackInterval = 1;    //攻击间隔
    [SerializeField] protected float detectRange = 1;       //侦测范围
    [SerializeField] protected float attackRange = 1;       //攻击范围
    [SerializeField] protected float maxHp = 1;             //最大生命值
    [SerializeField] protected int maxTarget = 2;           //最大目标个数
    [SerializeField] protected int cost = 0;                //费用
    [SerializeField] protected float combatValue = 0;       //战斗力
    [SerializeField] protected float cardCD = 0;            //召唤CD
    [SerializeField] protected float curHp = 1;
    [SerializeField] protected string entityType = null;
    public bool isEnemy = false;        //是否是敌人
    public int ID { get { return id; } }
    public int Cost { get { return cost; } }
    public string EntityType { get { return entityType; } }

    #endregion


    /// <summary>
    /// 设置信息
    /// </summary>
    public void SetData(int _id, string _entityName, float _atk, float _speed, float _attackInterval, float _detectRange,
                                float _attackRange, float _hp, int _cost, int _maxTarget, float _combatValue, float _cardCD)
    {
        id = _id;
        entityName = _entityName;
        atk = _atk;
        speed = _speed;
        attackInterval = _attackInterval;
        detectRange = _detectRange;
        attackRange = _attackRange;
        maxHp = _hp;
        cost = _cost;
        maxTarget = _maxTarget;
        combatValue = _combatValue;
        cardCD = _cardCD;
    }


    protected bool stop = false;
    protected Vector3 moveForward = Vector3.right;
    protected LayerMask enemyLayer;     //敌人图层
    protected GameObject model;         //单位模型
    protected GameObject enemyBase;     //敌人基地
    protected AIPath aiPath;            //A*寻路脚本
    protected Animator anim;



    #region  ----生命周期----

    protected virtual void Awake()
    {

    }

    protected bool hasInit = false;
    /// <summary>
    /// 初始化，较晚执行，置于fixupdate中
    /// </summary>
    protected virtual void Init()
    {

        model = transform.Find("model").gameObject;
        aiPath = GetComponent<AIPath>();
        anim = model.GetComponent<Animator>();

        if (isEnemy)
        {
            moveForward = Vector3.left;                                     //设置移动方向                  
            gameObject.layer = LayerMask.NameToLayer("EnemyEntity");        //设置单位图层
            enemyLayer = 1 << LayerMask.NameToLayer("MyEntity");            //设置敌人图层
            enemyBase = GameObject.Find("MyBase");
        }
        else
        {
            moveForward = Vector3.right;
            gameObject.layer = LayerMask.NameToLayer("MyEntity");            //设置单位图层
            enemyLayer = 1 << LayerMask.NameToLayer("EnemyEntity");          //设置敌人图层
            Transform tr = model.transform;
            tr.localScale = new Vector3(-tr.localScale.x, tr.localScale.y, tr.localScale.z);    //转向
            enemyBase = GameObject.Find("EnemyBase");
        }
        aiPath.maxSpeed = speed;
        curHp = maxHp;

        hasInit = true;
    }


    protected void FixedUpdate()
    {
        if (hasInit == false)
        {
            Init();
        }
        m_FixedUpdate();
    }

    protected virtual void m_FixedUpdate()
    {
        if (stop)
        {
            aiPath.maxSpeed = 0;
        }
        else
        {
            aiPath.maxSpeed = speed;
        }

        StopDetect();
        AttackDetect();

        if (curHp < 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion


    /// <summary>
    /// 停止移动检测，被敌人阻挡则停止
    /// </summary>
    protected virtual void StopDetect()
    {
        //射线检测
        RaycastHit[] hit = Physics.RaycastAll(transform.position - moveForward * 0.5f, moveForward, attackRange, enemyLayer);
        Debug.DrawRay(transform.position, moveForward * attackRange, Color.blue);

        if (hit.Length > 0)
        {
            stop = true;
        }
        else
        {
            stop = false;
        }
    }


    #region ----单位攻击----

    [SerializeField] protected List<Collider> targetCols = new List<Collider>();    //目标列表


    /// <summary>
    /// 敌人检测
    /// </summary>
    protected virtual void AttackDetect()
    {
        //圆形范围检测敌人
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        Debug.DrawRay(transform.position, Vector3.up * attackRange, Color.red);

        #region ----维护攻击目标列表----
        //从攻击列表中删除不存在的单位
        int tempCount = targetCols.Count;

        for (int j = tempCount; j > 0; j--)
        {
            //删除死亡的生物
            if (targetCols[j - 1] == null)
            {
                targetCols.Remove(targetCols[j - 1]);
            }
            else
            {
                bool exist = false;
                foreach (var col in cols)       //遍历当前列表
                {

                    if (targetCols[j - 1] == col)       //目标依然在范围内
                    {
                        exist = true;
                    }

                }
                if (!exist)
                {
                    targetCols.Remove(targetCols[j - 1]);     //目标走出范围，从攻击列表中删除目标
                }
            }


        }

        //遍历现阶段检测列表，并添加新攻击目标
        foreach (var col in cols)
        {
            //攻击目标已满，跳出
            if (targetCols.Count >= maxTarget)
            {
                break;
            }

            //排除已经在攻击列表的目标
            bool exist = false;
            foreach (var target in targetCols)      //遍历攻击列表
            {
                if (target == col)
                {
                    exist = true;
                    break;
                }
            }
            if (exist == false)                     //不存在，添加
            {
                targetCols.Add(col);
            }

        }
        #endregion

        //执行攻击命令
        if (targetCols.Count > 0)
        {
            foreach (var col in targetCols)
            {
                Attack(col.gameObject);

            }
        }
    }


    [SerializeField] protected float atkTimer;
    /// <summary>
    /// 进行攻击
    /// </summary>
    /// <param name="target"></param>
    protected virtual void Attack(GameObject target)
    {
        //攻击间隔计时
        atkTimer -= Time.deltaTime;
        //执行攻击
        if (atkTimer < 0)
        {
            target.GetComponent<IDoDamage>().DoDamage(atk);
            //播放动画
            anim.SetTrigger("atk");
            
            atkTimer = attackInterval;
        }
    }

    #endregion


    /// <summary>
    /// 伤害特效
    /// </summary>
    Image hpBar;
    IEnumerator HurtEffect()
    {
        model.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(0.5f);
        model.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //实现接口，对单位造成伤害
    public bool DoDamage(float value)
    {
        curHp -= value;

        if (hpBar == null)
        {
            hpBar = model.transform.Find("EntityUI").Find("hpBar").GetComponent<Image>();
        }
        hpBar.fillAmount = curHp / maxHp;

        StartCoroutine(HurtEffect());
        if (curHp < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
