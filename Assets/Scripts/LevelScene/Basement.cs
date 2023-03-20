using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basement : MonoBehaviour, IDoDamage
{

    public float maxHp;
    [SerializeField]
    private float curHp;
    public float CurHp { get { return curHp; } }
    public bool isEnemy = false;
    private void Awake()
    {
        Init();
    }
    protected void Init()
    {
        curHp = maxHp;
    }

    private void FixedUpdate()
    {
        m_FixedUpdate();
    }
    protected void m_FixedUpdate()
    {
        if (curHp < 0)
        {
            if (isEnemy)
            {
                FightManager.Instance.GameVectory();
            }
            else
            {
                FightManager.Instance.GameLoss();
            }

            //Destroy(gameObject);
        }
    }

    public bool DoDamage(float value)
    {
        curHp -= value;
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
