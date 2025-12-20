using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField] private int howManyTypes;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color[] colors;
    [SerializeField] private Sprite match4powerup;
    [SerializeField] private Sprite match5powerup;
    [SerializeField] private Sprite match5CrossPowerup;


    public void RandomizeMatchable(Matchable matchable)
    {
        int howManyTypes = Random.Range(0, this.howManyTypes);
        matchable.SetData(howManyTypes, sprites[howManyTypes], colors[howManyTypes]);
    }
    public Matchable GetPooledObjectRandom()
    {
        Matchable matchable = GetPooledObject();
        RandomizeMatchable(matchable);
        return matchable;
    }

    public int NextType(Matchable matchable)
    {
        int nextType = (matchable.Type + 1) % howManyTypes;
        matchable.SetData(nextType, sprites[nextType], colors[nextType]);
        return nextType;
    }
    public Matchable UpgradeMatchable(Matchable matchable, MatchType type)
    {
        if (type == MatchType.crossMatch)
            return matchable.Upgrade(match5CrossPowerup,type);

        if (type == MatchType.match4)
            return matchable.Upgrade(match4powerup,type);

        if (type == MatchType.match5)
            return matchable.Upgrade(match5powerup, type);



        return matchable;
    }

    public void ChangeType(Matchable match, int number)
    {
        match.SetData(number, sprites[number], colors[number]);
    }
}
