using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;
    public static T Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    protected void Awake()
    {
        if (instance == null) 
        {
            instance=this as T;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (instance ==this)
            instance=null;
    }

    protected virtual void Init() { }
}
