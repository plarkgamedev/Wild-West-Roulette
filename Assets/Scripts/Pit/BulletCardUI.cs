using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletCardUI : MonoBehaviour
{
    [SerializeField] Match3Config colors;      // rarity colors
    [SerializeField] RectTransform root;       // the card itself
    [SerializeField] Image header;             // colored title bar
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text rarityLine;
    [SerializeField] TMP_Text stats;
    [SerializeField] TMP_Text effect;
    [SerializeField] TMP_Text footer;
    [SerializeField] Vector2 offset = new Vector2(40f, 0f);

    BulletInstance current;

    public void Show(BulletInstance b, Vector3 screenPos, string footerText, Color footerColor)
    {
        root.anchorMin = root.anchorMax = Vector2.zero;
        root.pivot = new Vector2(0f, 0.5f);
        root.gameObject.SetActive(true);

        if (b != current)
        {
            current = b;
            var tint = colors.Tint(b.Rarity);
            header.color = tint;
            title.text = b.DisplayName;
            rarityLine.text = b.Rarity.ToString();
            rarityLine.color = tint;
            stats.text = $"Damage  x{b.DamageMultiplier:0.##}\nWeight  {b.Weight}";
            effect.text = string.IsNullOrEmpty(b.EffectName) ? "" : $"<b>{b.EffectName}</b>\n{b.EffectDescription}";
            effect.gameObject.SetActive(!string.IsNullOrEmpty(b.EffectName));
            footer.text = footerText;
            footer.color = footerColor;
            //LayoutRebuilder.ForceRebuildImmediate(root);
        }
        footer.text = footerText;
        footer.color = footerColor;

        // Keep the card on screen.
        float w = root.rect.width * root.lossyScale.x;
        float h = root.rect.height * root.lossyScale.y;
        float x = Mathf.Clamp(screenPos.x + offset.x, 0f, Mathf.Max(0f, Screen.width - w));
        float y = Mathf.Clamp(screenPos.y + offset.y, h * 0.5f, Mathf.Max(h * 0.5f, Screen.height - h * 0.5f));
        root.position = new Vector3(x, y, 0f);
    }

    public void Hide()
    {
        current = null;
        root.gameObject.SetActive(false);
    }
}