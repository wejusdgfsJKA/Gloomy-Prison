using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //player input component, used for miscellaneous stuff
    protected PlayerInput playerInput;
    [Header("Movement")]
    [SerializeField] protected float walkSpeed;
    [Header("Jumping")]
    [SerializeField] protected bool grounded;
    [SerializeField] protected float jumpHeight;
    protected Vector2 WASD;
    protected CharacterController characterController;
    [Header("Look")]
    [SerializeField] protected float maxVerticalAngle;
    [SerializeField] protected Transform camPivot;
    protected float xRot, yRot;
    protected Vector2 mouseDelta;
    //interaction stuff
    protected Transform currentInteractable = null;
    protected Transform currentInteractableProperty
    {
        get
        {
            return currentInteractable;
        }
        set
        {
            if (currentInteractable != value)
            {
                if (value == null)
                {
                    interactionPrompt.enabled = false;
                }
                else
                {
                    interactionPrompt.enabled = true;
                }
                currentInteractable = value;
            }
        }
    }
    RaycastHit hit;
    [SerializeField] TextMeshProUGUI interactionPrompt;
    protected void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }
    protected void OnEnable()
    {
        UpdateInteractionPrompt();
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
    public void UpdateInteractionPrompt()
    {
        interactionPrompt.text = "[" + playerInput.currentActionMap.
            FindAction("Interact").GetBindingDisplayString() + "] to interact.";
    }
    protected void InteractionCheck()
    {
        if (Physics.Raycast(camPivot.position, camPivot.forward, out hit, PlayerSettings.interactionDistance, 1 << 5))
        {
            if (InteractableManager.Instance.IsInteractable(hit.transform))
            {
                currentInteractableProperty = hit.transform;
                return;
            }
        }
        currentInteractableProperty = null;
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
        yRot += mouseDelta.x * PlayerSettings.ySens;
        xRot -= mouseDelta.y * PlayerSettings.xSens;
        xRot = Mathf.Clamp(xRot, -maxVerticalAngle, maxVerticalAngle);
        camPivot.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
        InteractionCheck();
    }
    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //SceneManager.LoadScene(0);
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && currentInteractable != null)
        {
            //attempt to interact with something in front of us
            InteractableManager.Instance.Interact(currentInteractable, transform);
        }
    }
}
