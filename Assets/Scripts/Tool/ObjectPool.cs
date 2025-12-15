using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
   [SerializeField] protected T prefab;

    private List<T> poolObjectList;
    private int poolAmount;
    private bool isReady;

    public void PoolObjects(int amount=0)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException("Amount to pool must be non negative");

        poolAmount = amount;
        poolObjectList = new List<T>(poolAmount);

        for (int i = 0; i < amount; i++) 
        {
            var newObject = Instantiate(prefab.gameObject,transform);
            newObject.SetActive(false);
            poolObjectList.Add(newObject.GetComponent<T>());
        }
        isReady= true;
    }

    public T GetPooledObject()
    {
        if (!isReady)
            PoolObjects(1);

        for (int i = 0; i != poolAmount; ++i)
        {
            if (!poolObjectList[i].isActiveAndEnabled)
                return poolObjectList[i];
        }

        var newObject = Instantiate(prefab.gameObject, transform);
        newObject.SetActive(false);
        poolObjectList.Add(newObject.GetComponent<T>());
        poolAmount++;
        return newObject.GetComponent<T>();
    }

    public void ReturnToPool(T toBeReturned)
    {
        if (toBeReturned == null) return;

        if (!isReady)
        {
            PoolObjects();
            poolObjectList.Add(toBeReturned);
        }

        toBeReturned.gameObject.SetActive(false);
    }

}
