using System;
using System.Collections.Generic;
using UnityEngine;

public enum Orientation
{
    none,
    horizontal,
    vertical,
    both
}

public enum MatchType
{
    none,
    match3,
    match4,
    match5,
    crossMatch
}
public class Match
{
    private int unlisted;

    private Matchable tobeUpgraded = null;

    private List<Matchable> matchables;
    public List<Matchable> Matchables { get { return matchables; } }
    public int Count { get { return matchables.Count + unlisted; } }

    public Orientation orientation = Orientation.none;

    private MatchType matchType = MatchType.none;

    public bool isCrossMatch = false;

    public MatchType Type
    {
        get
        {
            if (orientation == Orientation.both)
                return MatchType.crossMatch;

            else if (matchables.Count > 4)
                return MatchType.match5;

            else if (matchables.Count == 4)
                return MatchType.match4;

            else if (matchables.Count == 3)
                return MatchType.match3
                    ;
            else
            {
                return MatchType.none;
            }
        }
    }

    public Matchable GetToBeUpgradedMatchable()
    {
        if (tobeUpgraded != null)
            return tobeUpgraded;

        return matchables[UnityEngine.Random.Range(0, matchables.Count)];
    }

    public Match()
    {
        matchables = new List<Matchable>(5);
    }
    public Match(Matchable original) : this()
    {
        AddtoMatchables(original);
        tobeUpgraded = original;
    }
    public void AddtoMatchables(Matchable original)
    {
        matchables.Add(original);
    }
    public void AddUnlisted()
    {
        unlisted++;
    }

    public bool Contains(Matchable matchable)
    {
        return matchables.Contains(matchable);
    }

    public override string ToString()
    {
        string s = "(" + matchables[0].Type + ")";
        foreach (Matchable matchable in matchables)
        {
            s += "(" + matchable.position.x + "," + matchable.position.y + ")";
        }
        return s;
    }

    public void Merge(Match match)
    {
        matchables.AddRange(match.matchables);

        if (orientation == Orientation.both || match.orientation == Orientation.both
            || (orientation == Orientation.horizontal) && (match.orientation == Orientation.vertical)
            || (orientation == Orientation.vertical) && (match.orientation == Orientation.horizontal))
                orientation = Orientation.both;
        
        else if(match.orientation==Orientation.horizontal)
        {
            orientation = Orientation.horizontal;
        }
        else if (match.orientation == Orientation.vertical)
        {
            orientation = Orientation.vertical;
        }
    }

    public void Remove(Matchable powerup)
    {
        matchables.Remove(powerup);
    }
}
