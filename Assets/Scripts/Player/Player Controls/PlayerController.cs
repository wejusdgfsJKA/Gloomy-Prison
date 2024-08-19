using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected float walkSpeed;
    [SerializeField]
    bool grounded;
    protected Vector2 WASD;
    [SerializeField]
    protected CharacterController characterController;
    [Header("Look")]
    [SerializeField]
    protected float xSens, ySens;
    [SerializeField]
    float maxVerticalAngle;
    [SerializeField]
    Transform cam;
    float xRot, yRot;
    protected Vector2 mouseDelta;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        xRot = yRot = 0;
    }
    private void Update()
    {
        grounded = groundCheck();
        Vector3 movedir = transform.forward * WASD.y + transform.right * WASD.x;
        characterController.Move(movedir.normalized * walkSpeed * Time.deltaTime);
    }
    bool groundCheck()
    {
        return false;
    }
    protected void OnWalk(InputValue value)
    {
        WASD = value.Get<Vector2>();
    }
    protected void OnLook(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();
        yRot += mouseDelta.x * ySens;
        xRot -= mouseDelta.y * xSens;
        xRot = Mathf.Clamp(xRot, -maxVerticalAngle, maxVerticalAngle);
        cam.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
