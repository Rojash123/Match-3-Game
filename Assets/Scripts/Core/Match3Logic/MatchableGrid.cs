using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class MatchableGrid : GridSystem<Matchable>
{
    private MatchablePool pool;
    private ScoreManager scoreManager;


    private Vector3 onScreenPosition;

    [SerializeField] private Vector3 offSetScreen;

    private void Start()
    {
        pool = (MatchablePool)MatchablePool.Instance;
        scoreManager = ScoreManager.Instance;
    }
    public IEnumerator PopulateGrid(bool allowMatches = false, bool initialPopulation = false)
    {
        List<Matchable> newMatchables = new List<Matchable>();
        Matchable matchable;

        for (int i = 0; i < dimensions.y; ++i)
        {
            for (int j = 0; j < dimensions.x; ++j)
            {
                if (IsEmpty(j, i))
                {
                    matchable = pool.GetPooledObjectRandom();
                    matchable.position = new Vector2Int(j, i);
                    matchable.transform.position = transform.position + new Vector3(j, i) + offSetScreen;
                    matchable.gameObject.SetActive(true);
                    int type = matchable.Type;
                    while (!allowMatches && IsPartOfMatch(matchable))
                    {
                        if (pool.NextType(matchable) == type)
                        {
                            Debug.Log("failed to find a matchable type");
                            Debug.Break();
                            break;
                        }
                    }
                    AddDataToGrid(matchable, j, i);
                    newMatchables.Add(matchable);
                    //StartCoroutine(matchable.MoveCoroutine(onScreenPosition));
                    yield return null;
                }
            }
        }
        for (int i = 0; i < newMatchables.Count; i++)
        {
            onScreenPosition = transform.position + new Vector3(newMatchables[i].position.x, newMatchables[i].position.y);
            if (i == newMatchables.Count - 1)
            {
                yield return StartCoroutine(newMatchables[i].MoveCoroutine(onScreenPosition));
            }
            else
            {
                StartCoroutine(newMatchables[i].MoveCoroutine(onScreenPosition));
            }
            if (initialPopulation)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
    private bool IsPartOfMatch(Matchable matchable)
    {
        int horizontalMatches = 0,
            verticalMatches = 0;

        horizontalMatches += CountMatchesInDirection(matchable, Vector2Int.left);
        horizontalMatches += CountMatchesInDirection(matchable, Vector2Int.right);

        if (horizontalMatches > 1) return true;

        verticalMatches += CountMatchesInDirection(matchable, Vector2Int.up);
        verticalMatches += CountMatchesInDirection(matchable, Vector2Int.down);

        if (verticalMatches > 1) return true;

        return false;
    }

    private int CountMatchesInDirection(Matchable matchable, Vector2Int direction)
    {
        Vector2Int position = matchable.position + direction;
        int count = 0;
        while (CheckBounds(position) && !IsEmpty(position) && GetData(position).Type == matchable.Type)
        {
            count++;
            position += direction;
        }
        return count;
    }

    public IEnumerator TrySwap(Matchable[] toBeSwapped)
    {
        Matchable[] copies = new Matchable[2];
        copies[0] = toBeSwapped[0];
        copies[1] = toBeSwapped[1];

        yield return StartCoroutine(Swap(copies));
        if (copies[0].IsGem && copies[1].IsGem)
        {
            MatchEveryThing();
            yield break;
        }
        if (copies[0].IsGem)
        {
            MatchEveryThingByType(copies[0],copies[1].Type);
            yield break;
        }
        if (copies[1].IsGem)
        {
            MatchEveryThingByType(copies[1],copies[0].Type);
            yield break;
        }


        Match[] matches = new Match[2];
        matches[0] = GetMatch(copies[0]);
        matches[1] = GetMatch(copies[1]);

        if (matches[0] != null)
        {
            StartCoroutine(scoreManager.ResolveMatch(matches[0]));
        }

        if (matches[1] != null)
        {
            StartCoroutine(scoreManager.ResolveMatch(matches[1]));
        }

        if (matches[0] == null && matches[1] == null)
        {
            yield return StartCoroutine(Swap(copies));
            if (ScanGridForMatch())
            {
                StartCoroutine(FindAndScanGrid());
            }
        }
        else
        {
            StartCoroutine(FindAndScanGrid());
        }
        yield return null;
    }

    private void MatchEveryThingByType(Matchable gem,int type)
    {
        Match everythingSame = new();
        for (int i = 0; i < dimensions.y; i++)
        {
            for (int j = 0; j < dimensions.x; j++)
            {
                if (CheckBounds(j, i) && !IsEmpty(j, i) && GetData(j, i).Idle && type == GetData(j, i).Type)
                        everythingSame.AddtoMatchables(GetData(j, i));
            }
        }
        StartCoroutine(scoreManager.ResolveMatch(everythingSame, MatchType.match5));
        StartCoroutine(FindAndScanGrid());
    }

    private void MatchEveryThing()
    {
        Match everyThing = new();
        for (int i = 0; i < dimensions.y; i++)
        {
            for (int j = 0; j < dimensions.x; j++)
            {
                if (CheckBounds(j, i) && !IsEmpty(j, i) && GetData(j, i).Idle)
                    everyThing.AddtoMatchables(GetData(j, i));
            }
        }
        StartCoroutine(scoreManager.ResolveMatch(everyThing, MatchType.match5));
        StartCoroutine(FindAndScanGrid());
    }

    private IEnumerator FindAndScanGrid()
    {
        CollapSeGrid();
        yield return StartCoroutine(PopulateGrid(true));
        if (ScanGridForMatch())
        {
            StartCoroutine(FindAndScanGrid());
        }
    }

    private bool ScanGridForMatch()
    {
        bool madeMatch = false;
        Matchable toMatch;
        Match match;
        for (int i = 0; i < dimensions.y; i++)
        {
            for (int j = 0; j < dimensions.x; j++)
            {
                if (!IsEmpty(j, i))
                {
                    toMatch = GetData(j, i);

                    if (!toMatch.Idle) continue;

                    match = GetMatch(toMatch);
                    if (match != null)
                    {
                        madeMatch = true;
                        StartCoroutine(scoreManager.ResolveMatch(match));
                    }
                }
            }
        }
        return madeMatch;
    }

    private void CollapSeGrid()
    {
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                if (IsEmpty(i, j))
                {
                    for (int nonEmpty = j + 1; nonEmpty < dimensions.x; nonEmpty++)
                    {
                        if (!IsEmpty(i, nonEmpty) && GetData(i, nonEmpty).Idle)
                        {
                            MoveMatchable(GetData(i, nonEmpty), i, j);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void MoveMatchable(Matchable matchable, int x, int y)
    {
        MoveItemTo(matchable.position, new Vector2Int(x, y));
        matchable.position = new Vector2Int(x, y);

        StartCoroutine(matchable.MoveCoroutine(transform.position + new Vector3(x, y)));

    }

    private Match GetMatch(Matchable matchable)
    {
        Match match = new Match(matchable);

        Match horizontalMatch,
              verticalMatch;

        horizontalMatch = GetMatchesInDirection(match, matchable, Vector2Int.left);
        horizontalMatch.Merge(GetMatchesInDirection(match, matchable, Vector2Int.right));

        horizontalMatch.orientation = Orientation.horizontal;

        if (horizontalMatch.Count > 1)
        {
            match.Merge(horizontalMatch);
            GetBranches(match, horizontalMatch, Orientation.vertical);
        }


        verticalMatch = GetMatchesInDirection(match, matchable, Vector2Int.up);
        verticalMatch.Merge(GetMatchesInDirection(match, matchable, Vector2Int.down));

        verticalMatch.orientation = Orientation.vertical;


        if (verticalMatch.Count > 1)
        {
            match.Merge(verticalMatch);
            GetBranches(match, verticalMatch, Orientation.horizontal);
        }

        if (match.Count == 1) return null;
        return match;
    }

    private void GetBranches(Match tree, Match branchToSearch, Orientation perpendicular)
    {
        Match branch;
        foreach (Matchable matchable in branchToSearch.Matchables)
        {
            branch = GetMatchesInDirection(tree, matchable, perpendicular == Orientation.horizontal ? Vector2Int.left : Vector2Int.down);
            branch.Merge(GetMatchesInDirection(tree, matchable, perpendicular == Orientation.horizontal ? Vector2Int.right : Vector2Int.up));
            branch.orientation = perpendicular;

            if (branch.Count > 1)
            {
                tree.Merge(branch);
                GetBranches(tree, branch, perpendicular == Orientation.horizontal ? Orientation.vertical : Orientation.horizontal);
            }
        }
    }

    private Match GetMatchesInDirection(Match tree, Matchable matchable, Vector2Int direction)
    {
        Match match = new Match();
        Vector2Int position = matchable.position + direction;
        Matchable next;
        while (CheckBounds(position) && !IsEmpty(position))
        {
            next = GetData(position);
            if (next.Type == matchable.Type && next.Idle)
            {
                if (!tree.Contains(next))
                {
                    match.AddtoMatchables(next);
                }
                else
                {
                    match.AddUnlisted();
                }
                position += direction;
            }
            else break;
        }
        return match;
    }

    private IEnumerator Swap(Matchable[] toBeSwapped)
    {
        SwapItem(toBeSwapped[0].position, toBeSwapped[1].position);

        Vector2Int tempPos = toBeSwapped[0].position;
        toBeSwapped[0].position = toBeSwapped[1].position;
        toBeSwapped[1].position = tempPos;

        Vector3[] worldPosition = new Vector3[2];
        worldPosition[0] = toBeSwapped[0].transform.position;
        worldPosition[1] = toBeSwapped[1].transform.position;

        StartCoroutine(toBeSwapped[0].MoveCoroutine(worldPosition[1]));
        yield return StartCoroutine(toBeSwapped[1].MoveCoroutine(worldPosition[0]));
    }

    public void MatchAllAdjacent(Matchable powerUp)
    {
        Match allAdjacent = new();
        for (int i = powerUp.position.y - 1; i < powerUp.position.y + 2; i++)
        {
            for (int j = powerUp.position.x - 1; j < powerUp.position.x + 2; j++)
            {
                if (CheckBounds(j, i) && !IsEmpty(j, i) && GetData(j, i).Idle)
                {
                    allAdjacent.AddtoMatchables(GetData(j, i));
                }
            }
        }
        StartCoroutine(scoreManager.ResolveMatch(allAdjacent, MatchType.match4));
    }

    public void MatchRowAndColumn(Matchable powerUp)
    {
        Match allRowAndColumn = new();
        for (int i = 0; i < dimensions.y; i++)
        {
            if (CheckBounds(powerUp.position.x, i) && !IsEmpty(powerUp.position.x, i) && GetData(powerUp.position.x, i).Idle)
                allRowAndColumn.AddtoMatchables(GetData(powerUp.position.x, i));
        }
        for (int i = 0; i < dimensions.x; i++)
        {
            if (CheckBounds(i, powerUp.position.y) && !IsEmpty(i, powerUp.position.y) && GetData(i, powerUp.position.y).Idle)
            {
                allRowAndColumn.AddtoMatchables(GetData(i, powerUp.position.y));
            }
        }
        StartCoroutine(scoreManager.ResolveMatch(allRowAndColumn, MatchType.crossMatch));
    }

}
