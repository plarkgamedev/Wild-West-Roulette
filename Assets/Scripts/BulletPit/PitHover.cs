using UnityEngine;
using UnityEngine.InputSystem;

public class PitHover : MonoBehaviour
{
    [SerializeField] PitConfig config;
    [SerializeField] Camera cam;
    [SerializeField] LayerMask bulletMask;
    [SerializeField] BulletCardUI card;
    [SerializeField] PitLoadout loadout;
    [SerializeField] PitManager manager;
    [SerializeField] CylinderView cylinder;
    [SerializeField] InputActionReference interact;

    PitBullet hovered;

    void OnEnable()
    {
        interact.action.Enable();
        interact.action.performed += OnInteract;
    }

    void OnDisable() => interact.action.performed -= OnInteract;

    void Update()
    {
        hovered = null;
        if (cylinder.IsOpen) return;   // the cylinder screen owns the card while it is open

        if (manager.Active)
        {
            var ray = new Ray(cam.transform.position, cam.transform.forward);
            if (Physics.Raycast(ray, out var hit, config.pickupRange, bulletMask, QueryTriggerInteraction.Ignore))
                hovered = hit.collider.GetComponentInParent<PitBullet>();
        }

        if (hovered == null) { card.Hide(); return; }

        Vector3 sp = cam.WorldToScreenPoint(hovered.transform.position + Vector3.up * 0.15f);
        if (sp.z < 0f) { card.Hide(); return; }

        bool ok = loadout.CanAdd(hovered.Data, out string reason);
        card.Show(hovered.Data, sp, ok ? "E   Pick up" : reason,
                  ok ? Color.white : new Color(1f, 0.35f, 0.3f));
    }

    void OnInteract(InputAction.CallbackContext _)
    {
        if (!manager.Active || cylinder.IsOpen || hovered == null) return;
        loadout.TryAdd(hovered);
        hovered = null;
        card.Hide();
    }
}