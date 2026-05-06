using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 1f;        // Altezza da accovacciato
    [SerializeField] private float standingHeight = 2f;      // Altezza normale
    [SerializeField] private float crouchSpeed = 2.5f;       // Velocità mentre sei accovacciato
    private bool isCrouching = false;

    [Header("Look Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 20f;
    private float xRotation = 0f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip crouchSound;
    [SerializeField] private float footstepInterval = 0.5f;
    private float footstepTimer;

    private PlayerControls inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;
    private float defaultCameraY;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
        defaultCameraY = playerCamera.transform.localPosition.y;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Update()
    {
        HandleMovement();
        HandleLook();
        HandleCrouch();
        if (inputActions.Player.Interact.triggered)
        {
            PerformInteraction();
        }
    }

    void HandleCrouch()
    {
        // Toggle dello stato
        isCrouching = inputActions.Player.Crouch.IsPressed();

        float targetHeight = isCrouching ? crouchHeight : standingHeight;

        float cameraOffset = (standingHeight - crouchHeight) * 0.5f; // Calcola quanto deve spostarsi la camera
        float targetCameraY = isCrouching ? (defaultCameraY - cameraOffset) : defaultCameraY; // Regola in base alla "faccia" del tuo player

        // Interpola l'altezza del controller
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 5f);

        // Interpola la posizione della camera
        Vector3 newCamPos = playerCamera.transform.localPosition;
        newCamPos.y = Mathf.Lerp(newCamPos.y, targetCameraY, Time.deltaTime * 5f);
        playerCamera.transform.localPosition = newCamPos;
        controller.center = new Vector3(0, controller.height / 2f, 0);

        if (inputActions.Player.Crouch.triggered) // Quando premi CTRL
        {
            footstepSource.PlayOneShot(crouchSound);
        }
    }

    // --- FUNZIONE CORRETTA ---
    public void ResetRotationFromCurrentOrientation()
    {
        // 1. Sincronizziamo la rotazione verticale (X)
        // Usiamo playerCamera.transform perché localEulerAngles appartiene al Transform
        float currentX = playerCamera.transform.localEulerAngles.x;

        // Normalizziamo l'angolo: Unity usa 0-360, ma xRotation usa -90 a 90
        if (currentX > 180) currentX -= 360;
        xRotation = currentX;

        // 2. Per la rotazione orizzontale (Y):
        // Nel tuo HandleLook usi transform.Rotate(Vector3.up * mouseX), 
        // quindi non hai bisogno di una variabile yRotation. 
        // Il corpo è già ruotato, dobbiamo solo assicurarci che HandleLook 
        // continui da lì (cosa che fa già perché transform.Rotate è relativo).
    }

    void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        isSprinting = inputActions.Player.Sprint.IsPressed();

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * Time.deltaTime * currentSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        }
    

    void HandleLook()
    {
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotazione verticale (Camera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotazione orizzontale (Corpo)
        transform.Rotate(Vector3.up * mouseX);
    }

    void PerformInteraction()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}