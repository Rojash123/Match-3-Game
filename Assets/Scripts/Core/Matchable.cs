using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Matchable : Movable
{
    private SpriteRenderer spriteRenderer;
    private int type;
    private Cursor cursor;

    public int Type {  get { return type; } }

    public Vector2 position;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cursor = Cursor.Instance;
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

    public void SetData(int type,Sprite sprite, Color color)
    {
        this.type = type;
        spriteRenderer.sprite = sprite; 
        spriteRenderer.color=color;
    }
    
}
