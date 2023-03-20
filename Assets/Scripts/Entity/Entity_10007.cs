using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_10007 : Entity_dear
{
    public enum State
    {
        Idle,
        Rush,
    }
    private float rushTimer;
    [SerializeField] private float rushTimerMax = 3;
    [SerializeField] private float rushAttackMultiplying = 2f;
    [SerializeField] private float rushSpeedMultiplying = 1.5f;
    [SerializeField] private State state;
    [SerializeField] private float secondAttackCoefficient = 0.6f;
    private float multipyedSpeed;
    private float unMultipyedSpeed;
    protected override void Init()//重载初始化，初始化状态机
    {
        base.Init();
        state = State.Idle;
        rushTimer = rushTimerMax;
        multipyedSpeed = speed * rushSpeedMultiplying;
        unMultipyedSpeed = speed;
    }

    protected override void m_FixedUpdate()
    {
        base.m_FixedUpdate();
        switch (state)
        {
            case State.Idle:
                rushTimer -= Time.deltaTime;
                if (rushTimer <= 0)
                {
                    state = State.Rush;
                    rushTimer = rushTimerMax;
                }
                speed = unMultipyedSpeed;
                break;
            case State.Rush:
                speed = multipyedSpeed;
                break;

        }
    }

    protected override void AttackDetect()
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

        if (targetCols.Count > 0)//添加了第一个百分之一百伤害，后续百分之六十伤害
        {
            int count = 1;
            float currentAtk = atk;
            foreach (var col in targetCols)
            {

                if (count == 1)
                {
                    Attack(col.gameObject);
                }
                else
                {
                    atk = currentAtk * secondAttackCoefficient;
                    Attack(col.gameObject);
                }

            }
            atk = currentAtk;

        }
    }

    protected override void Attack(GameObject target)
    {
        atkTimer -= Time.deltaTime;
        bool killEntity = false;
        //执行攻击
        if (atkTimer < 0)
        {
            if (state == State.Rush)
            {
                killEntity = target.GetComponent<IDoDamage>().DoDamage(atk * rushAttackMultiplying);
                if (killEntity)
                {
                    OnWhenKill();
                }
                state = State.Idle;
            }
            else
            {
                killEntity = target.GetComponent<IDoDamage>().DoDamage(atk);
                if (killEntity)
                {
                    OnWhenKill();
                }
            }
            atkTimer = attackInterval;
        }
    }

    private void CureAllDearCurrently()
    {
        foreach (var dear in needCureDearList)
        {
            dear.GetComponent<Entity_dear>().CureSelf();
        }
    }
    

}
