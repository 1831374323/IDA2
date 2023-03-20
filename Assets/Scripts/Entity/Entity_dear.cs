using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity_dear : Entity
{
    public event Action WhenKill;
    public event Action WhenSpawn;
    public static List<Entity_dear> needCureDearList;
    [SerializeField] private float curePercent=0.5f; 

    protected override void Awake()
    {
        base.entityType = "dear";
        needCureDearList = new List<Entity_dear>();
    }

    protected override void Init()
    {
        base.Init();
    }

    protected override void Attack(GameObject target)
    {
        //攻击间隔计时
        atkTimer -= Time.deltaTime;
        bool killEntity = false;
        //执行攻击
        if (atkTimer < 0)
        {
            killEntity = target.GetComponent<IDoDamage>().DoDamage(atk);
            if (killEntity)
            {
                WhenKill?.Invoke();
            }
            atkTimer = attackInterval;
        }
    }

    protected virtual void OnWhenKill()
    {
        WhenKill?.Invoke();
    }

    public virtual void CureSelf()
    {
        if (this.curHp == maxHp)
        {
            return;
        }
        else
        {
            curHp += (maxHp - curHp)*curePercent ;//
            //这边需要治疗动画
        }
    }
}
