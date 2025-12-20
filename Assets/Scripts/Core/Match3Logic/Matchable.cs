using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{
    private MatchablePool pool;
    private MatchableGrid grid;

    private SpriteRenderer spriteRenderer;
    private int type;
    private Cursor cursor;
    private MatchType powerUpType=MatchType.none;

    public bool IsGem
    {
        get { return powerUpType == MatchType.match5; }
    }

    public int Type { get { return type; } }

    public Vector2Int position;

    public int SortingOrder
    {
        set { spriteRenderer.sortingOrder = value; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cursor = Cursor.Instance;
        pool = (MatchablePool)MatchablePool.Instance;
        grid = (MatchableGrid)MatchableGrid.Instance;
    }


    private void OnMouseDown()
    {
        cursor.SelectFirstMatchable(this);
    }

    private void OnMouseUp()
    {
        cursor.SelectFirstMatchable(null);
    }

    private void OnMouseEnter()
    {
        cursor.SelectSecondMatchable(this);
    }

    public void SetData(int type, Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }
    public IEnumerator Resolve(Transform collectPoint)
    {
        if (powerUpType != MatchType.none) 
        {
            if(powerUpType == MatchType.match4)
            {
                grid.MatchAllAdjacent(this);
            }

            if (powerUpType == MatchType.match5)
            {
            }

            if (powerUpType == MatchType.crossMatch)
            {
                grid.MatchRowAndColumn(this);
            }
            powerUpType = MatchType.none;
        }
        if (collectPoint == null)
        {
            yield break;
        }

        spriteRenderer.sortingOrder = 2;
        yield return StartCoroutine(MoveCoroutine(collectPoint));
        spriteRenderer.sortingOrder = 1;
        pool.ReturnToPool(this);
    }

    public Matchable Upgrade(Sprite match4powerup,MatchType type)
    {
        if (type != MatchType.none)
        {
            idle = false;
            StartCoroutine(Resolve(null));
            idle = true;
        }

        powerUpType = type;
        if (powerUpType == MatchType.match5)
        {
            this.type = -1;
            spriteRenderer.color=Color.white;
        }
        spriteRenderer.sprite = match4powerup;
        return this;
    }
}
