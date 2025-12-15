using UnityEngine;

public class MatchablePool : ObjectPool<Matchable>
{
    [SerializeField]private int howManyTypes;
    [SerializeField]private Sprite[] sprites;
    [SerializeField]private Color[] colors;

    public void RandomizeMatchable(Matchable matchable)
    {
        howManyTypes=Random.Range(0, howManyTypes);
        matchable.SetData(howManyTypes, sprites[howManyTypes], colors[howManyTypes]);
    }
    public Matchable GetPooledObjectRandom()
    {
        Matchable matchable=GetPooledObject();
        RandomizeMatchable(matchable);
        return matchable;
    }

    public int NextType(Matchable matchable)
    {
        int nextType=(matchable.Type+1)% howManyTypes;
        matchable.SetData(nextType, sprites[nextType], colors[nextType]);
        return nextType;
    }
}
