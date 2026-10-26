using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] InputActionReference interact;
    [SerializeField] GameObject prompt;   // a small "E" sprite above the player
    IInteractable current;

    void OnEnable() { interact.action.Enable(); interact.action.performed += OnInteract; }
    void OnDisable() { interact.action.performed -= OnInteract; }

    void OnInteract(InputAction.CallbackContext _) => current?.Interact();

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.TryGetComponent(out IInteractable i)) SetCurrent(i);
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.TryGetComponent(out IInteractable i) && i == current) SetCurrent(null);
    }

    void SetCurrent(IInteractable i)
    {
        current = i;
        if (prompt) prompt.SetActive(i != null);
    }
}