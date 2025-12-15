using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Cursor : Singleton<Cursor>
{
    private SpriteRenderer spriteRenderer;
    private Matchable[] selected;

    private Vector2 horizontalStretch = new(2, 1);
    private Vector2 verticalStretch = new(1, 2);

    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        selected = new Matchable[2];
    }

    public void SelectFirstMatchable(Matchable matchable)
    {
        selected[0] = matchable;
        if (!enabled || selected[0] == null) return;

        this.transform.position = matchable.transform.position;
        spriteRenderer.enabled = true;
    }
    public void SelectSecondMatchable(Matchable matchable)
    {
        selected[1] = matchable;
        if (!enabled || selected[0] == null || selected[1] == null || !selected[0].Idle || !selected[1].Idle || selected[0] == selected[1]) return;
        if (!selectedAdjacent()) return;

        selected[0] = null;
    }

    private bool selectedAdjacent()
    {
        if (selected[0].position.x == selected[1].position.x)
        {
            if (selected[0].position.y == selected[1].position.y + 1)
            {
                spriteRenderer.size = verticalStretch;
                transform.position += Vector3.down / 2;
                return true;
            }
            if (selected[0].position.y == selected[1].position.y - 1)
            {
                spriteRenderer.size = verticalStretch;
                transform.position += Vector3.up / 2;
                return true;
            }
        }
        else if (selected[0].position.y == selected[1].position.y)
        {
            if (selected[0].position.x == selected[1].position.x + 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += Vector3.left / 2;
                return true;
            }
            if (selected[0].position.x == selected[1].position.x - 1)
            {
                spriteRenderer.size = horizontalStretch;
                transform.position += Vector3.right / 2;
                return true;
            }
        }

        return false;
    }
}
