using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] Transform collectPosition;
    [SerializeField] Slider comboSlider;

    private MatchableGrid grid;
    private MatchablePool pool;

    [SerializeField]
    private TextMeshProUGUI scoreText,
                 comboText;

    private int score;


    private int comboMultiplier=0;
    private float maxComboDuration=5, comboDuration;
    private float lastTimeCombo;
    private bool isComboActive=false;

    public int Score
    {
        get { return score; }
    }
    protected override void Init()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        grid = (MatchableGrid)MatchableGrid.Instance;
        pool = (MatchablePool)MatchablePool.Instance;
    }
    public void AddScore(int score)
    {
        int combo = IncreaseCombo();
        this.score += score*combo;
        scoreText.text = $"Score:\n{this.score}";
        comboText.text = $"Combo X {comboMultiplier}";
    }

    private int IncreaseCombo()
    {
        lastTimeCombo = 0;
        comboDuration =maxComboDuration-Mathf.Log(comboMultiplier);
        
        if (!isComboActive)
            StartCoroutine(ComboTimer());

        return ++comboMultiplier;
    }

    IEnumerator ComboTimer()
    {
        isComboActive = true;
        comboSlider.gameObject.SetActive(true);
        do 
        {
            lastTimeCombo += Time.deltaTime;
            comboSlider.value = 1-lastTimeCombo/comboDuration;
            yield return null;
        }
        while (lastTimeCombo < comboDuration);

        comboSlider.gameObject.SetActive(false);
        comboMultiplier = 0;
        isComboActive = false;
    }


    public IEnumerator ResolveMatch(Match matches, MatchType powerUpUsed = MatchType.none)
    {
        Matchable matchable;
        Matchable powerup = null;

        Transform target = collectPosition;

        if (powerUpUsed == MatchType.none && matches.Count > 3)
        {
            powerup = pool.UpgradeMatchable(matches.GetToBeUpgradedMatchable(), matches.Type);
            matches.Remove(powerup);
            target = powerup.transform;
            powerup.SortingOrder = 5;
        }

        for (int i = 0; i < matches.Count; ++i)
        {
            matchable = matches.Matchables[i];

            if (powerUpUsed != MatchType.match5 && matchable.IsGem)
                continue;

            grid.RemoveDataFromGrid(matchable.position);

            if (i == matches.Count - 1)
            {
                yield return StartCoroutine(matchable.Resolve(target));
            }
            else
            {
                StartCoroutine(matchable.Resolve(target));
            }
        }
        AddScore(matches.Count * matches.Count);

        if (powerup != null)
            powerup.SortingOrder = 1;

        yield return null;
    }
}
