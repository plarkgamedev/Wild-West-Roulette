using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CylinderUI : MonoBehaviour
{
    [SerializeField] PitLoadout loadout;
    [SerializeField] Match3Config colors;
    [SerializeField] Image[] chamberIcons;       // one per chamber, in firing order
    [SerializeField] Image weightFill;           // Image Type = Filled
    [SerializeField] TMP_Text weightText;
    [SerializeField] Color normalBar = new Color(0.9f, 0.75f, 0.3f);
    [SerializeField] Color fullBar = new Color(0.9f, 0.3f, 0.25f);

    void Awake()
    {
        if (!loadout) Debug.LogError($"CylinderUI on '{name}': the Loadout field is empty. Drag PitSystems into it.", this);
        if (!colors) Debug.LogError($"CylinderUI on '{name}': the Colors field is empty. Drag your Match3 Config into it.", this);
        if (chamberIcons == null || chamberIcons.Length == 0)
            Debug.LogError($"CylinderUI on '{name}': Chamber Icons is empty. Set the size to 6 and drag the chamber Images in.", this);
    }

    void OnEnable()
    {
        if (loadout) loadout.Changed += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (loadout) loadout.Changed -= Refresh;
    }

    void Refresh()
    {
        if (!loadout || !colors) return;

        var loaded = loadout.Loaded;
        for (int i = 0; i < chamberIcons.Length; i++)
        {
            var img = chamberIcons[i];
            if (!img) { Debug.LogWarning($"CylinderUI: Chamber Icons element {i} is empty.", this); continue; }

            bool filled = i < loaded.Count;
            img.gameObject.SetActive(filled);          // turns the whole object on or off
            if (!filled) continue;

            img.enabled = true;
            img.sprite = loaded[i].Icon;
            var c = colors.Tint(loaded[i].Rarity);
            c.a = 1f;                                   // never invisible because of a transparent tint
            img.color = c;
            img.preserveAspect = true;
        }

        int w = loadout.Weight, limit = loadout.Limit;
        if (weightFill)
        {
            weightFill.fillAmount = limit > 0 ? Mathf.Clamp01(w / (float)limit) : 0f;
            weightFill.color = w >= limit ? fullBar : normalBar;
        }
        if (weightText) weightText.text = $"Weight {w}/{limit}";
    }
}