using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //定义是否全局单例，全局单例启用 DontDestroyOnLoad
    [SerializeField] private bool global = true;

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (global)
        {
            // 如果实例已存在，且不等于当前对象，销毁当前对象，使用原来的实例，以维持单例
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            // 如果实例为空，要重新赋值
            instance = this.gameObject.GetComponent<T>();
            DontDestroyOnLoad(this.gameObject);
        }
        //保证执行顺序
        this.OnAwake();
    }

    /// <summary>
    /// 跟 Unity 的 Awake() 一个用法，因为实现单例模式占用了原本的 Awake()，子类可以通过覆写 OnAwake() 来使用 Awake()
    /// </summary>
    protected virtual void OnAwake()
    { }
}