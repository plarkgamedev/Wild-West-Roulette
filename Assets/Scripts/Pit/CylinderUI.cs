using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CylinderUI : MonoBehaviour
{
    [SerializeField] Match3Config colors;
    [SerializeField] Image[] chamberIcons;       // one per chamber, laid over the holes of your cylinder art
    [SerializeField] Image weightFill;           // Image Type = Filled
    [SerializeField] TMP_Text weightText;
    [SerializeField] Color normalBar = new Color(0.9f, 0.75f, 0.3f);
    [SerializeField] Color fullBar = new Color(0.9f, 0.3f, 0.25f);

    public void Refresh(IReadOnlyList<BulletInstance> loaded, int weight, int limit)
    {
        for (int i = 0; i < chamberIcons.Length; i++)
        {
            var img = chamberIcons[i];
            if (i < loaded.Count)
            {
                img.enabled = true;
                img.sprite = loaded[i].Icon;
                img.color = colors.Tint(loaded[i].Rarity);
                img.preserveAspect = true;
            }
            else img.enabled = false;
        }

        weightFill.fillAmount = limit > 0 ? Mathf.Clamp01(weight / (float)limit) : 0f;
        weightFill.color = weight >= limit ? fullBar : normalBar;
        weightText.text = $"Weight {weight}/{limit}";
    }
}