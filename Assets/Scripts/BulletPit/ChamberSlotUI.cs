using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChamberSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image back;           // the hole. Raycast Target ON, so empty slots still receive the mouse
    [SerializeField] Image icon;           // the bullet. Raycast Target OFF
    [SerializeField] TMP_Text weightLabel; // optional: this bullet's weight, shown in the corner
    [SerializeField] Color normal = new Color(0f, 0f, 0f, 0.55f);
    [SerializeField] Color highlight = new Color(1f, 0.85f, 0.3f, 0.85f);

    public int Index { get; set; }
    public BulletInstance Bullet { get; private set; }

    public event Action<ChamberSlotUI> Entered;
    public event Action<ChamberSlotUI> Exited;
    public event Action<ChamberSlotUI, PointerEventData.InputButton> Clicked;

    public void Set(BulletInstance b, Color tint)
    {
        Bullet = b;
        icon.enabled = b != null;

        if (weightLabel)
        {
            weightLabel.gameObject.SetActive(b != null);
            if (b != null) weightLabel.text = b.Weight.ToString();
        }

        if (b == null) return;
        icon.sprite = b.Icon;
        icon.color = tint;
        icon.preserveAspect = true;
    }

    public void SetHover(bool on) => back.color = on ? highlight : normal;

    public void OnPointerEnter(PointerEventData e) => Entered?.Invoke(this);
    public void OnPointerExit(PointerEventData e) => Exited?.Invoke(this);
    public void OnPointerClick(PointerEventData e) { if (Bullet != null) Clicked?.Invoke(this, e.button); }
}