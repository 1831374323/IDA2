using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_10001 : Entity_dear
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
    private float multipyedSpeed;
    private float unMultipyedSpeed;
    protected override void Init()//重载初始化，初始化状态机
    {
        base.Init();
        state = State.Idle;
        rushTimer = rushTimerMax;
        multipyedSpeed = speed * rushSpeedMultiplying;
        unMultipyedSpeed = speed;
        needCureDearList.Add(this);//把自己加入治疗列表
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
                    rushTimer=rushTimerMax;
                }
                speed = unMultipyedSpeed;
                anim.SetBool("rush",false);
                break;
            case State.Rush:
                speed = multipyedSpeed;
                anim.SetBool("rush",true);
                break;

        }
    }
    protected override void AttackDetect()
    {
      base.AttackDetect();
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
                killEntity = target.GetComponent<IDoDamage>().DoDamage(atk*rushAttackMultiplying);
                if (killEntity)
                {
                    OnWhenKill();
                }
                state = State.Idle;
            }
            else{
                killEntity = target.GetComponent<IDoDamage>().DoDamage(atk);
                if (killEntity)
                {
                    OnWhenKill();
                }
            }       
            atkTimer = attackInterval;
        }
        anim.SetTrigger("atk");
    }

    protected override void OnWhenKill()//因为子类不能直接调用基类的事件，所以在此嵌套
    {
        base.OnWhenKill();
    }

    
}
