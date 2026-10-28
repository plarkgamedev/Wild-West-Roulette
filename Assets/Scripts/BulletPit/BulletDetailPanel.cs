using System;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put this on the full-screen overlay. Clicking anywhere on it closes it.
public class BulletDetailPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Match3Config colors;
    [SerializeField] Image header;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text rarityLine;
    [SerializeField] Image bigIcon;
    [SerializeField] TMP_Text statsText;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text notesText;
    [SerializeField] TMP_Text footer;

    public bool IsOpen { get; private set; }
    Action onClose;

    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public void Show(BulletInstance b, int position, int chambers, int weightUsed, int weightLimit, Action closed)
    {
        onClose = closed;
        IsOpen = true;
        gameObject.SetActive(true);

        var tint = colors.Tint(b.Rarity);
        header.color = tint;
        title.text = b.DisplayName;
        rarityLine.text = b.Rarity.ToString();
        rarityLine.color = tint;
        bigIcon.sprite = b.Icon;
        bigIcon.color = tint;
        bigIcon.preserveAspect = true;

        float pct = (b.DamageMultiplier - 1f) * 100f;
        var s = new StringBuilder();
        s.AppendLine($"<b>Damage  x{b.DamageMultiplier.ToString("0.##", Inv)}</b>  ({pct.ToString("+0;-0;0", Inv)}%)");
        s.AppendLine($"Your base damage is multiplied by {b.DamageMultiplier.ToString("0.##", Inv)} when this bullet hits.");
        s.AppendLine();
        s.AppendLine($"<b>Weight  {b.Weight}</b>");
        s.AppendLine($"Uses {b.Weight} of your cylinder's {weightLimit} weight. You are carrying {weightUsed}/{weightLimit}. Dropping it frees {b.Weight}.");
        s.AppendLine();
        s.AppendLine($"<b>Chamber  {position} of {chambers}</b>");
        s.Append("You choose the firing order. The game spins the starting chamber before the duel.");
        statsText.text = s.ToString();

        var e = new StringBuilder();
        if (string.IsNullOrEmpty(b.EffectName))
            e.AppendLine("<b>No special effect.</b> This bullet only deals its damage.");
        else
        {
            e.AppendLine($"<b>Special effect: {b.EffectName}</b>");
            if (!string.IsNullOrEmpty(b.EffectDescription)) e.AppendLine(b.EffectDescription);
        }
        e.AppendLine();
        e.AppendLine($"<b>Before</b> <size=80%>(you shoot first)</size>: {OrNone(b.BeforeEffect)}");
        e.AppendLine($"<b>During</b> <size=80%>(the bullets collide)</size>: {OrNone(b.DuringEffect)}");
        e.Append($"<b>After</b> <size=80%>(the enemy shoots first)</size>: {OrNone(b.AfterEffect)}");
        effectText.text = e.ToString();

        string n = RarityNote(b.Rarity);
        if (!string.IsNullOrEmpty(b.FlavorText)) n += $"\n\n<i>{b.FlavorText}</i>";
        notesText.text = n;

        footer.text = "Click anywhere or press TAB to close";
    }

    public void Hide()
    {
        IsOpen = false;
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData e)
    {
        Hide();
        onClose?.Invoke();
    }

    static string OrNone(string s) => string.IsNullOrWhiteSpace(s) ? "<color=#888888>nothing</color>" : s;

    static string RarityNote(Rarity r) => r switch
    {
        Rarity.White => "White bullets are the lightest and weakest. Cheap filler that leaves room for heavier picks.",
        Rarity.Blue => "Blue bullets are uncommon: stronger than white and a little heavier.",
        Rarity.Purple => "Purple bullets are rare: noticeably stronger and heavy to carry.",
        _ => "Legendary bullets are the strongest and heaviest. Your weight limit only allows a couple."
    };
}