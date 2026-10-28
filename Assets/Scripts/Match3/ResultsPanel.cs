using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        run.PitPercentages = config.PitChances(run.PitReelCounts);
        run.PitPool = null;   // forces a fresh roll the next time the pit is entered

        StartCoroutine(Reveal(result, onContinue));
    }

    IEnumerator Reveal(Rarity[] result, Action onContinue)
    {
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
                v.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, k * (2f - k));
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
    var pct = run.PitPercentages;
    var baseChance = config.PitBase;
    var inv = CultureInfo.InvariantCulture;

    var sb = new StringBuilder();
    sb.AppendLine("<b>Bullet pit odds</b>");
    for (int i = 3; i >= 0; i--)   // best rarity first
    {
        string hex = ColorUtility.ToHtmlStringRGB(config.Tint((Rarity)i));
        float delta = pct[i] - baseChance[i];
        string change = Mathf.Abs(delta) < 0.05f ? ""
            : $"  <size=80%>({(delta > 0 ? "+" : "")}{delta.ToString("0.#", inv)})</size>";
        sb.AppendLine($"<color=#{hex}>{(Rarity)i}  {pct[i].ToString("0.#", inv)}%</color>{change}");
    }
    return sb.ToString();
}
}