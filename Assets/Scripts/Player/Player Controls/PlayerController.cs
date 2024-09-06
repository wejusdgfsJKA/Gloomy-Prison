using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float walkSpeed;
    [Header("Jumping")]
    [SerializeField] protected bool grounded;
    [SerializeField] protected float jumpHeight;
    protected Vector2 WASD;
    protected CharacterController characterController;
    [Header("Look")]
    [SerializeField] protected float xSens;
    [SerializeField] protected float ySens;
    [SerializeField] protected float maxVerticalAngle;
    [SerializeField] protected Transform cam;
    protected float xRot, yRot;
    protected Vector2 mouseDelta;
    protected void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    protected void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        xRot = yRot = 0;
    }
    protected void Update()
    {
        grounded = groundCheck();
        Vector3 movedir = transform.forward * WASD.y + transform.right * WASD.x;
        characterController.Move(movedir.normalized * walkSpeed * Time.deltaTime);
    }
    protected bool groundCheck()
    {
        return false;
    }
    public void OnWalk(InputAction.CallbackContext context)
    {
        WASD = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
        yRot += mouseDelta.x * ySens;
        xRot -= mouseDelta.y * xSens;
        xRot = Mathf.Clamp(xRot, -maxVerticalAngle, maxVerticalAngle);
        cam.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SceneManager.LoadScene(0);
        }
    }
}
