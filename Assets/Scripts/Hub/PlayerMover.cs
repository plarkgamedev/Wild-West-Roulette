using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] InputActionReference move;
    Rigidbody2D rb;
    Vector2 input;

    void Awake() => rb = GetComponent<Rigidbody2D>();
    void OnEnable() => move.action.Enable();
    void OnDisable() => move.action.Disable();
    void Update() => input = Vector2.ClampMagnitude(move.action.ReadValue<Vector2>(), 1f);
    void FixedUpdate() => rb.linearVelocity = input * speed; // Unity 6; older versions: rb.velocity
}