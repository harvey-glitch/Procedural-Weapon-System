using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField, Tooltip("Normal walking speed of the player.")]
    public float movementSpeed = 4.0f;

    [SerializeField, Tooltip("How far the player can look up or down.")]
    public float rotationAngleLimit = 65.0f;

    [SerializeField, Tooltip("How fast the camera rotate around.")]
    public float rotationSpeed = 3.0f;

    [SerializeField, Tooltip("Makes gravity stronger or weaker.")]
    public float gravityMultiplier = 2.0f;

    [Space(5)]
    [Header("Ground Check Settings")]

    [SerializeField, Tooltip("Point used to check if the player is on the ground.")]
    public Transform checkSphere;

    [SerializeField, Tooltip("Size of the sphere used for checking ground contact.")]
    public float radius = 0.5f;

    [SerializeField, Tooltip("Offset checking for ground, useful for stairs.")]
    public float groundedOffset = 0.14f;

    [SerializeField, Tooltip("Shows if the player is touching the ground.")]
    public bool isGrounded;

    // Reference to the CharacterController component for movement
    private CharacterController _characterController;

    // Reference to the main camera for handling rotations
    private Camera _camera;

    // Stores the current vertical and horizontal angles for rotation
    private float _pitch, _yaw;

    // Tracks the player's current vertical velocity (used for gravity and jumping)
    private Vector3 _verticalVelocity;

    private void Awake()
    {
        _characterController ??= GetComponent<CharacterController>();
        _camera = Camera.main;
    }

    private void Start()
    {
        // hide the cursor at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GroundCheck();

        HandleMove();
        HandleRotate();
        ApplyGravity();
    }

    private void HandleMove()
    {
        // normalize the input to avoid faster movement when moving diagonally
        Vector3 movementInput = GetMovementInput().normalized * movementSpeed;

        // only move when theres a valid input
        if (movementInput.sqrMagnitude >= 0.0001f)
        {
            // create a direction based on orientation and input
            Vector3 movement = transform.right * movementInput.x + transform.forward * movementInput.z;

            _characterController.Move(movement * Time.deltaTime);
        }
    }

    private void HandleRotate()
    {
        Vector2 rotationInput = GetRotationInput() * rotationSpeed;

        _yaw += rotationInput.x;  // accumulate horizontal rotation turning
        _pitch -= rotationInput.y; // accumulate vertical rotation for looking up and down

        // clamp the vertical rotation to avoid camera flipping
        _pitch = Mathf.Clamp(_pitch, -rotationAngleLimit, rotationAngleLimit);

        // apply the horizontal and vertical rotation
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void GroundCheck()
    {
        Vector3 sphereOrigin = checkSphere.position + Vector3.down * groundedOffset;

        // check if the character is grounded
        isGrounded = Physics.CheckSphere(sphereOrigin, radius);
    }

    private void ApplyGravity()
    {
        // push the character to the ground
        if (isGrounded && _verticalVelocity.y < 0)
        {
            _verticalVelocity.y = -2f;
        }

        if (!isGrounded)
        {
            // only apply gravity when not grounded
            _verticalVelocity.y += -9.81f * gravityMultiplier * Time.deltaTime;
        }

        _characterController.Move(_verticalVelocity * Time.deltaTime);
    }

    #region Input Methods
    private Vector3 GetMovementInput()
    {
        return new Vector3(
            Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    private Vector3 GetRotationInput()
    {
        return new Vector2(
            Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
    #endregion

    #region For Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 spherOrigin = checkSphere.position + Vector3.down * groundedOffset;

        Gizmos.DrawWireSphere(spherOrigin, radius);
    }
    #endregion
}