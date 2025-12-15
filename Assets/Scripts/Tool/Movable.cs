using System;
using System.Collections;
using UnityEngine;

public class Movable : MonoBehaviour
{
    private Vector3 from, to;
    private float howFar;
    
    [SerializeField]
    private float speed=1;

    private bool idle;
    public bool Idle
    {
        get
        {
            return idle;
        }
    }
    public IEnumerator MoveCoroutine(Vector3 targetPosition)
    {
        from=transform.position;
        to= targetPosition;
        howFar = 0;
        idle = false;
        do
        {
            howFar+=speed*Time.deltaTime;
            if (howFar > 1) howFar = 1;
            transform.position=Vector3.LerpUnclamped(from, to, Easing(howFar));
            yield return null;
        }
        while (howFar!=1) ;
        idle = true;
    }

    private float Easing(float t)
    {
        return t*t;
    }
}
