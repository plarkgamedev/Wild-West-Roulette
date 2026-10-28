using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CylinderView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] PitConfig config;
    [SerializeField] Match3Config colors;
    [SerializeField] PitLoadout loadout;
    [SerializeField] PitManager manager;
    [SerializeField] PitPlayer player;

    [Header("UI")]
    [SerializeField] GameObject panel;           // the cylinder screen
    [SerializeField] GameObject crosshair;       // hidden while the screen is open
    [SerializeField] ChamberSlotUI[] slots;      // in firing order
    [SerializeField] Image weightFill;           // Image Type = Filled
    [SerializeField] TMP_Text weightText;        // the BIG "TOTAL WEIGHT" readout
    [SerializeField] Color normalBar = new Color(0.9f, 0.75f, 0.3f);
    [SerializeField] Color fullBar = new Color(0.9f, 0.3f, 0.25f);
    [SerializeField] BulletCardUI card;
    [SerializeField] BulletDetailPanel detail;

    [Header("Input")]
    [SerializeField] InputActionReference toggle;

    [Header("Drop position (relative to the player's feet)")]
    [SerializeField] float dropForward = 0.6f;
    [SerializeField] float dropHeight = 0.5f;
    [SerializeField] float dropSpread = 0.25f;

    ChamberSlotUI hovered;

    public bool IsOpen { get; private set; }
    public bool DetailOpen => detail.IsOpen;

    void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Index = i;
            slots[i].Entered += s => hovered = s;
            slots[i].Exited += s => { if (hovered == s) hovered = null; };
            slots[i].Clicked += OnSlotClicked;
        }
        panel.SetActive(false);
        detail.Hide();
    }

    void OnEnable()
    {
        toggle.action.Enable();
        toggle.action.performed += OnToggle;
        loadout.Changed += OnLoadoutChanged;
    }

    void OnDisable()
    {
        toggle.action.performed -= OnToggle;
        loadout.Changed -= OnLoadoutChanged;
    }

    void OnDestroy() { Time.timeScale = 1f; }   // never leave the game frozen

    void OnLoadoutChanged() { if (IsOpen) Refresh(); }

    void OnToggle(InputAction.CallbackContext _)
    {
        if (IsOpen) Close();
        else if (manager.Active) Open();
    }

    public void Open()
    {
        IsOpen = true;
        panel.SetActive(true);
        if (crosshair) crosshair.SetActive(false);
        card.Hide();
        player.SetUiMode(true);
        Refresh();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        detail.Hide();
        IsOpen = false;
        hovered = null;
        panel.SetActive(false);
        card.Hide();
        if (crosshair) crosshair.SetActive(true);
        player.SetUiMode(false);
    }

    void Update()
    {
        if (!IsOpen) return;
        if (detail.IsOpen) { card.Hide(); return; }

        foreach (var s in slots) s.SetHover(s == hovered && s.Bullet != null);

        if (hovered != null && hovered.Bullet != null)
        {
            var rt = (RectTransform)hovered.transform;
            Vector2 sp = RectTransformUtility.WorldToScreenPoint(null, rt.position);
            sp.x += rt.rect.width * rt.lossyScale.x * 0.5f;
            card.Show(hovered.Bullet, sp, "Left click: drop     Right click: details", Color.white);
        }
        else card.Hide();
    }

    void OnSlotClicked(ChamberSlotUI slot, PointerEventData.InputButton button)
    {
        if (detail.IsOpen || slot.Bullet == null) return;

        if (button == PointerEventData.InputButton.Left) Drop(slot.Index);
        else if (button == PointerEventData.InputButton.Right) ShowDetail(slot.Index);
    }

    void Drop(int index)
    {
        var t = player.transform;
        Vector3 pos = t.position + t.forward * dropForward
                      + t.right * Random.Range(-dropSpread, dropSpread)
                      + Vector3.up * dropHeight;
        loadout.DropAt(index, pos);   // raises Changed, which refreshes this screen
        card.Hide();
    }

    void ShowDetail(int index)
    {
        var b = loadout.Loaded[index];
        card.Hide();
        Time.timeScale = 0f;
        detail.Show(b, index + 1, config.chambers, loadout.Weight, loadout.Limit, () => Time.timeScale = 1f);
    }

    void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var b = i < loadout.Loaded.Count ? loadout.Loaded[i] : null;
            slots[i].Set(b, b != null ? colors.Tint(b.Rarity) : Color.white);
        }

        int w = loadout.Weight, limit = loadout.Limit;
        int free = Mathf.Max(0, limit - w);
        bool full = w >= limit;

        if (weightFill)
        {
            weightFill.fillAmount = limit > 0 ? Mathf.Clamp01(w / (float)limit) : 0f;
            weightFill.color = full ? fullBar : normalBar;
        }

        if (weightText)
        {
            string hex = full ? "#FF5A4D" : "#FFFFFF";
            weightText.text = $"<size=50%>TOTAL WEIGHT</size>\n<color={hex}>{w} / {limit}</color>  " +
                              $"<size=50%>{(full ? "FULL" : free + " free")}</size>";
        }
    }
}