using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultSlotView : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text label;

    public void Set(Rarity r, Sprite sprite, Color tint)
    {
        icon.sprite = sprite;
        icon.color = tint;
        icon.preserveAspect = true;
        label.text = r.ToString();
        label.color = tint;
    }
}