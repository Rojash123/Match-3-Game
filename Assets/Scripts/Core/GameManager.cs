using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField] private Vector2Int dimension;

    private void Start()
    {
        pool = (MatchablePool)MatchablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;

        StartCoroutine(Demo());
    }
    IEnumerator Demo()
    {
        pool.PoolObjects(dimension.x*dimension.y*2);
        grid.Initialize(dimension);
        yield return null;
        StartCoroutine(grid.PopulateGrid());
    }
}
