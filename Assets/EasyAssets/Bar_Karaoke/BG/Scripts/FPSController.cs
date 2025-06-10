using UnityEngine;

public class FPSController : MonoBehaviour
{
    // --- Variables de Movimiento ---
    [Header("Movimiento")]
    public float moveSpeed = 5f; // Velocidad de movimiento
    public float jumpForce = 8f; // Fuerza de salto
    public float gravity = -20f; // Gravedad (negativa para ir hacia abajo)

    private CharacterController controller;
    private Vector3 velocity; // Velocidad actual del personaje

    // --- Variables de Vista de C�mara ---
    [Header("Vista de C�mara")]
    public float mouseSensitivity = 100f; // Sensibilidad del mouse
    public Transform playerBody; // Transform del cuerpo del jugador (la c�psula)
    public Transform cameraTransform; // Transform de la c�mara

    private float xAxisClamp = 0.0f; // Para limitar la rotaci�n vertical de la c�mara

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // Aseg�rate de que el playerBody es este mismo objeto
        if (playerBody == null)
        {
            playerBody = transform;
        }
        // Aseg�rate de que cameraTransform es la c�mara hija
        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        // Bloquear el cursor y hacerlo invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- Movimiento del Personaje ---
        // Detectar entradas de teclado (WASD)
        float x = Input.GetAxis("Horizontal"); // 'A' y 'D'
        float z = Input.GetAxis("Vertical");   // 'W' y 'S'

        // Calcular direcci�n de movimiento relativa al jugador (hacia donde mira)
        Vector3 moveDirection = playerBody.right * x + playerBody.forward * z;
        moveDirection.Normalize(); // Normalizar para que el movimiento diagonal no sea m�s r�pido

        // Aplicar velocidad de movimiento
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Aplicar gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Peque�a fuerza para asegurar que est� en el suelo
        }

        // Salto
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = jumpForce;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // --- Control de la C�mara (Vista en Primera Persona) ---
        // Obtener el movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotar el cuerpo del jugador horizontalmente (eje Y)
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotar la c�mara verticalmente (eje X), limitando los �ngulos
        xAxisClamp -= mouseY; // Restar porque mover el mouse hacia arriba debe bajar el valor del �ngulo
        xAxisClamp = Mathf.Clamp(xAxisClamp, -90f, 90f); // Limitar entre -90 y 90 grados

        cameraTransform.localRotation = Quaternion.Euler(xAxisClamp, 0f, 0f);
    }
}