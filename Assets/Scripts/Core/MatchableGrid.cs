using System;
using System.Collections;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    private MatchablePool pool;
    private Vector3 onScreenPosition;

    [SerializeField] private Vector3 offSetScreen;

    private void Start()
    {
        pool = (MatchablePool)MatchablePool.Instance;
    }
    public IEnumerator PopulateGrid(bool allowMatches=false)
    {
        Matchable matchable;
        for (int i = 0; i <dimensions.y; ++i)
        {
            for (int j = 0; j < dimensions.x; ++j)
            {
                matchable = pool.GetPooledObjectRandom();
                onScreenPosition = transform.position + new Vector3(j, i);
                matchable.position = new Vector3(j, i);
                matchable.transform.position=onScreenPosition+ offSetScreen;
                matchable.gameObject.SetActive(true);
                AddDataToGrid(matchable,j,i);
                int type = matchable.Type;
                while(!allowMatches && IsPartOfMatch(matchable))
                {
                    if(pool.NextType(matchable)== type)
                    {
                        Debug.Log("failed to find a matchable type");
                        Debug.Break();
                        break;
                    }
                }
                StartCoroutine(matchable.MoveCoroutine(onScreenPosition));
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private bool IsPartOfMatch(Matchable matchable)
    {
        return false;
    }
}
