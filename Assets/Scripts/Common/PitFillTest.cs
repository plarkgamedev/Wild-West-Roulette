using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class PitFillTest : MonoBehaviour
{
    [SerializeField] Match3Config config;
    [SerializeField] TMP_Text readout;

    [Header("Used only when you press Play directly in this scene")]
    [SerializeField] int debugPitSize = 100;
    [SerializeField] float[] debugPercentages = { 50f, 16.7f, 16.7f, 16.6f };   // White, Blue, Purple, Legendary

    void Start()
    {
        var run = GameManager.Instance.Run;
        bool real = run.PitPercentages != null;
        var pct = real ? run.PitPercentages : debugPercentages;
        int size = real ? run.PitSize : debugPitSize;

        if (run.PitPool == null)
            run.PitPool = PitOdds.RollPool(pct, size, new System.Random());

        var actual = new int[4];
        foreach (var r in run.PitPool) actual[(int)r]++;

        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.AppendLine($"Pit filled: {run.PitPool.Count} bullets");
        sb.AppendLine();
        for (int i = 3; i >= 0; i--)
        {
            string hex = ColorUtility.ToHtmlStringRGB(config.Tint((Rarity)i));
            float expected = pct[i] * size / 100f;
            sb.AppendLine($"<color=#{hex}>{(Rarity)i}: {actual[i]}</color>   (expected about {expected.ToString("0", inv)})");
        }
        readout.text = sb.ToString();
    }
}