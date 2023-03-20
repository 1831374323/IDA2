using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_10010 : Entity_dear
{

    [SerializeField] private float secondAttackCoefficient = 0.6f;

    protected override void Init()
    {
        base.Init();
        needCureDearList.Add(this);//���Լ����������б�
    }

    protected override void Attack(GameObject target)
    {
        //���������ʱ
        atkTimer -= Time.deltaTime;
        bool killEntity = false;
        //ִ�й���
        if (atkTimer < 0)
        {
            killEntity = target.GetComponent<IDoDamage>().DoDamage(atk);
            if (killEntity)
            {
                OnWhenKill();
            }
            atkTimer = attackInterval;
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
        { int count = 1;
          float currentAtk=atk;
            foreach (var col in targetCols)
            {
                
                if (count == 1)
                {
                    Attack(col.gameObject);
                }
                else
                {
                    atk=currentAtk*secondAttackCoefficient;
                    Attack(col.gameObject);
                }

            }
            atk=currentAtk;

        }
    }


}
