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
    protected override void Init()//���س�ʼ������ʼ��״̬��
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
        //Բ�η�Χ������
        Collider[] cols = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        Debug.DrawRay(transform.position, Vector3.up * attackRange, Color.red);

        #region ----ά������Ŀ���б�----
        //�ӹ����б���ɾ�������ڵĵ�λ
        int tempCount = targetCols.Count;

        for (int j = tempCount; j > 0; j--)
        {
            //ɾ������������
            if (targetCols[j - 1] == null)
            {
                targetCols.Remove(targetCols[j - 1]);
            }
            else
            {
                bool exist = false;
                foreach (var col in cols)       //������ǰ�б�
                {

                    if (targetCols[j - 1] == col)       //Ŀ����Ȼ�ڷ�Χ��
                    {
                        exist = true;
                    }

                }
                if (!exist)
                {
                    targetCols.Remove(targetCols[j - 1]);     //Ŀ���߳���Χ���ӹ����б���ɾ��Ŀ��
                }
            }


        }

        //�����ֽ׶μ���б�������¹���Ŀ��
        foreach (var col in cols)
        {
            //����Ŀ������������
            if (targetCols.Count >= maxTarget)
            {
                break;
            }

            //�ų��Ѿ��ڹ����б��Ŀ��
            bool exist = false;
            foreach (var target in targetCols)      //���������б�
            {
                if (target == col)
                {
                    exist = true;
                    break;
                }
            }
            if (exist == false)                     //�����ڣ����
            {
                targetCols.Add(col);
            }

        }
        #endregion

        if (targetCols.Count > 0)//����˵�һ���ٷ�֮һ���˺��������ٷ�֮��ʮ�˺�
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
        //ִ�й���
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
