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

    [Header("Look Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivity = 20f;
    private float xRotation = 0f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerControls inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerControls();

        // Blocca il cursore al centro dello schermo
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Update()
    {
        HandleMovement();
        HandleLook();
        if (inputActions.Player.Interact.triggered) // Se premi E
        {
            PerformInteraction();
        }
    }

    void HandleMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Un piccolo valore negativo aiuta a stare incollati al suolo
        }

        // Leggi Input Movimento e Sprint
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        isSprinting = inputActions.Player.Sprint.IsPressed(); // Verifica se il tasto sprint è premuto

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Muoviti nella direzione in cui guarda il player
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * Time.deltaTime * currentSpeed);

        // Gravità
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void HandleLook()
    {
        // Leggi l'input del mouse
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotazione verticale (Camera) - Clamp per non girare la testa di 360 gradi
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotazione orizzontale (Corpo del Player)
        transform.Rotate(Vector3.up * mouseX);
    }

    void PerformInteraction()
    {
        // Crea un raggio che parte dal centro della camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // Controlla se l'oggetto colpito ha uno script che usa IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}