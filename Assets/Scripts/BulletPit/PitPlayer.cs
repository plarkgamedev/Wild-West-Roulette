using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PitPlayer : MonoBehaviour
{
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference look;
    [SerializeField] Transform cameraPivot;
    [SerializeField] float speed = 4f;
    [SerializeField] float lookSensitivity = 0.1f;
    [SerializeField] float gravity = -20f;

    CharacterController cc;
    float pitch;
    float yVel;

    public bool Locked { get; set; }

    void Awake() => cc = GetComponent<CharacterController>();

    void OnEnable()
    {
        move.action.Enable();
        look.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // true = menu mode (free cursor, no walking or looking). false = back to first person.
    public void SetUiMode(bool ui)
    {
        Locked = ui;
        Cursor.lockState = ui ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = ui;
    }

    void Update()
    {
        if (Locked) return;

        Vector2 l = look.action.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, l.x, 0f);
        pitch = Mathf.Clamp(pitch - l.y, -80f, 80f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        Vector2 m = Vector2.ClampMagnitude(move.action.ReadValue<Vector2>(), 1f);
        Vector3 dir = transform.right * m.x + transform.forward * m.y;
        yVel = cc.isGrounded ? -1f : yVel + gravity * Time.deltaTime;
        cc.Move((dir * speed + Vector3.up * yVel) * Time.deltaTime);
    }
}