using System.Collections;
using UnityEngine;

public class HintIndicator : Singleton<HintIndicator>
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    public float delayBeforeHint;

    private Transform hintLocation;

    private Coroutine autoHintCoroutine;

    protected override void Init()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }
    public void StartAutoHint(Transform hintLocation)
    {
        this.hintLocation = hintLocation;

        if (autoHintCoroutine == null) 
            autoHintCoroutine=StartCoroutine(WaitAndIndicateHint());
    }
    public void IndicateHint(Transform hintLocation)
    {
        transform.position = hintLocation.position; 
        _spriteRenderer.enabled = true;
    }
    public void CancelHint()
    {
        _spriteRenderer.enabled = false;
        if (autoHintCoroutine != null)
            StopCoroutine(autoHintCoroutine);

        autoHintCoroutine = null;
    }
    private IEnumerator WaitAndIndicateHint()
    {
        yield return new WaitForSeconds(delayBeforeHint);
        IndicateHint(hintLocation);
    }
}
