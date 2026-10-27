using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : MonoBehaviour
{
    [SerializeField] Match3Config config;
    [SerializeField] Sprite bulletSprite;
    [SerializeField] RectTransform rowContainer;
    [SerializeField] ResultSlotView slotPrefab;
    [SerializeField] TMP_Text summaryText;
    [SerializeField] Button continueButton;
    [SerializeField] int pitSize = 100;
    [SerializeField] float revealDelay = 0.25f;
    [SerializeField] float popTime = 0.2f;

    public void Show(Rarity[] result, Action onContinue)
    {
        gameObject.SetActive(true);
        continueButton.gameObject.SetActive(false);
        summaryText.text = "";
        foreach (Transform t in rowContainer) Destroy(t.gameObject);

        // Do the math once and store it for the bullet pit.
        var run = GameManager.Instance.Run;
        run.PitSize = pitSize;
        run.PitReelCounts = PitOdds.CountRarities(result);
        run.PitPercentages = PitOdds.Percentages(run.PitReelCounts);
        run.PitPool = null;   // forces a fresh roll the next time the pit is entered

        StartCoroutine(Reveal(result, onContinue));
    }

    IEnumerator Reveal(Rarity[] result, Action onContinue)
    {
        // Create everything first so the layout group fixes every position,
        // then pop them in one at a time, left to right.
        var views = new List<ResultSlotView>();
        foreach (var r in result)
        {
            var v = Instantiate(slotPrefab, rowContainer);
            v.Set(r, bulletSprite, config.Tint(r));
            v.transform.localScale = Vector3.zero;
            views.Add(v);
        }

        foreach (var v in views)
        {
            for (float t = 0; t < popTime; t += Time.unscaledDeltaTime)
            {
                float k = t / popTime;
                v.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, k * (2f - k));   // ease out
                yield return null;
            }
            v.transform.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(revealDelay);
        }

        summaryText.text = BuildOddsText(GameManager.Instance.Run);
        continueButton.gameObject.SetActive(true);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => onContinue?.Invoke());
    }

    string BuildOddsText(RunState run)
    {
        var counts = run.PitReelCounts;
        int reels = counts.Sum();
        var inv = CultureInfo.InvariantCulture;

        var sb = new StringBuilder();
        sb.AppendLine($"Chance for each pit bullet ({reels} reels, {(100f / reels).ToString("0.0", inv)}% per reel)");
        sb.AppendLine();

        for (int i = counts.Length - 1; i >= 0; i--)   // best rarity first
        {
            string name = ((Rarity)i).ToString();
            if (counts[i] == 0)
            {
                sb.AppendLine($"<color=#777777>{name}: 0%  (none in this pit)</color>");
                continue;
            }
            string hex = ColorUtility.ToHtmlStringRGB(config.Tint((Rarity)i));
            int expected = Mathf.RoundToInt(run.PitPercentages[i] * run.PitSize / 100f);
            sb.AppendLine($"<color=#{hex}>{name}: {run.PitPercentages[i].ToString("0.0", inv)}%</color>   " +
                          $"{counts[i]} of {reels} reels, about {expected} of {run.PitSize} bullets");
        }
        return sb.ToString();
    }
}