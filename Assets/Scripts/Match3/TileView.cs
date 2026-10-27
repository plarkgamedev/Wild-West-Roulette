using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class TileView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject selectionFrame;

    public RectTransform Rect { get; private set; }
    Image image;
    CanvasGroup group;
    System.Action<TileView> onClick;

    void Awake()
    {
        Rect = (RectTransform)transform;
        image = GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
    }

    public void Init(Sprite sprite, Color tint, System.Action<TileView> click)
    {
        image.sprite = sprite;
        image.color = tint;            // the colored filter: tint multiplies the sprite's colors
        image.preserveAspect = true;
        onClick = click;
        SetSelected(false);
    }

    public void SetSelected(bool on) { if (selectionFrame) selectionFrame.SetActive(on); }
    public void SetDim(float alpha) => group.alpha = alpha;
    public void OnPointerClick(PointerEventData e) => onClick?.Invoke(this);
}