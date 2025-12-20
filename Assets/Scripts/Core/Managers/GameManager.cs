using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    private MatchablePool pool;
    private MatchableGrid grid;

    [SerializeField] private Vector2Int dimension;
    [SerializeField] private GameObject noMatchFoundPanel;

    private void Start()
    {
        pool = (MatchablePool)MatchablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;
        StartCoroutine(StartPopulatingGrid());
    }
    IEnumerator StartPopulatingGrid()
    {
        pool.PoolObjects(dimension.x*dimension.y*2);
        grid.Initialize(dimension);
        yield return null;
        StartCoroutine(grid.PopulateGrid(false,true));
        grid.CheckPossibleMoves();
    }

    public void NoMovesAvailable()
    {
        noMatchFoundPanel.SetActive(true);
        StartCoroutine(NoMatchFound());
    }

    IEnumerator NoMatchFound()
    {
        yield return new WaitForSeconds(3f);
        grid.MatchEveryThing();
        noMatchFoundPanel.SetActive(false);
    }
}
