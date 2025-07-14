using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Normal walking speed of the player.")]
    public float MovementSpeed = 4.0f;

    [Tooltip("Walking speed of the player when sprinting.")]
    public float SprintingSpeed = 8f;

    [Tooltip("How far the player can look up or down.")]
    public float RotationAngleLimit = 65.0f;

    [Tooltip("How fast the camera rotate around.")]
    public float RotationSpeed = 3.0f;

    [Tooltip("Makes gravity stronger or weaker.")]
    public float GravityMultiplier = 2.0f;

    [Space(5)]
    [Header("Ground Check Settings")]

    [Tooltip("Point used to check if the player is on the ground.")]
    public Transform SphereTransform;

    [Tooltip("Size of the sphere used for checking ground contact.")]
    public float SphereRadius = 0.5f;

    [Tooltip("Offset checking for ground, useful for stairs.")]
    public float GroundedOffset = 0.14f;

    [Tooltip("Shows if the player is touching the ground.")]
    public bool IsGrounded;

    // stores the current movement direction of the player
    [HideInInspector] public Vector3 moveVector;

    // reference to the input manager
    private InputManager _inputManager;

    // reference to the CharacterController component for movement
    private CharacterController _characterController;

    // reference to the main camera for handling rotations
    private Camera _camera;

    // stores the current vertical and horizontal angles for rotation
    private float _pitch, _yaw;

    // tracks the player's current vertical velocity (used for gravity and jumping)
    private Vector3 _verticalVelocity;

    // check if the player was moved forward
    private bool _movedForward;

    // the current speed of the player
    private float _currentSpeed;

    private void Awake()
    {
        _characterController ??= GetComponent<CharacterController>();
        _inputManager ??= FindFirstObjectByType<InputManager>();
        _camera = Camera.main;
    }

    private void Start()
    {
        // hide the cursor at the start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // set the current speed value to movement speed by default
        _currentSpeed = MovementSpeed;
}

    private void Update()
    {
        GroundCheck();
        HandleMovement();
        HandleRotation();
        HandleGravity();
    }

    private void HandleMovement()
    {
        // Cache the result to avoid multiple calls and ensure consistent behavior
        bool movingForward = IsMovingForward();

        // Only update the current speed when the movement direction state changes
        if (_movedForward != movingForward)
        {
            _movedForward = movingForward;
            _currentSpeed = movingForward ? SprintingSpeed : MovementSpeed;
        }

        Vector2 moveInput = _inputManager.MoveInput;

        if (moveInput.sqrMagnitude >= 0.0001f)
        {
            // create a direction based on orientation and input
            moveVector = transform.right * moveInput.x + transform.forward * moveInput.y;

            _characterController.Move(moveVector * _currentSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Vector2 lookInput = _inputManager.LookInput * RotationSpeed;

        _yaw += lookInput.x;  // accumulate horizontal rotation turning
        _pitch -= lookInput.y; // accumulate vertical rotation for looking up and down

        // clamp the vertical rotation to avoid camera flipping
        _pitch = Mathf.Clamp(_pitch, -RotationAngleLimit, RotationAngleLimit);

        // apply the horizontal and vertical rotation
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void GroundCheck()
    {
        Vector3 sphereOrigin = SphereTransform.position + Vector3.down * GroundedOffset;

        // check if the character is grounded
        IsGrounded = Physics.CheckSphere(sphereOrigin, SphereRadius);
    }

    private void HandleGravity()
    {
        // push the character to the ground
        if (IsGrounded && _verticalVelocity.y < 0)
        {
            _verticalVelocity.y = -2f;
        }

        if (!IsGrounded)
        {
            // only apply gravity when not grounded
            _verticalVelocity.y += -9.81f * GravityMultiplier * Time.deltaTime;
        }

        _characterController.Move(_verticalVelocity * Time.deltaTime);
    }

    public bool IsMovingForward()
    {
        // calculate the dot product between movement and forward direction
        float angle = Vector3.Dot(moveVector.normalized, transform.forward);
        return _inputManager.MoveInput.sqrMagnitude >= 0.001f && angle > 0.86f;
    }

    public bool IsMoving()
    {
        return _inputManager.MoveInput.sqrMagnitude >= 0.0001f;
    }

    #region For Debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 spherOrigin = SphereTransform.position + Vector3.down * GroundedOffset;

        Gizmos.DrawWireSphere(spherOrigin, SphereRadius);
    }
    #endregion
}