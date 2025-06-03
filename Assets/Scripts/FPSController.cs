using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    [Header("PLAYER")]
    [Tooltip("Normal walking speed of the character.")]
    public float moveSpeed = 4.0f;

    [Tooltip("How far the character can look up or down.")]
    public float lookLimit = 65.0f;

    [Tooltip("How fast the camera looks around.")]
    public float lookSpeed = 3.0f;

    [Tooltip("Makes gravity stronger or weaker.")]
    public float gravityMultiplier = 2.0f;

    [Header("GROUND CHECK")]

    [Tooltip("Point used to check if the player is on the ground.")]
    public Transform checkSphere;

    [Tooltip("Size of the sphere used for checking ground contact.")]
    public float radius = 0.5f;

    [Tooltip("Offset checking for ground, useful for stairs.")]
    public float groundedOffset = 0.14f;

    [Tooltip("Shows if the player is touching the ground.")]
    public bool isGrounded;

    // references
    private CharacterController _charController;
    private Camera _camera;

    // useful for smoothing input for movement and rotation
    private Vector3 _smoothMoveInput;
    private Vector2 _smoothLookInput;

    private Vector3 _rawMoveInput;
    private Vector3 _rawLookInput;

    // tracks the vertical rotation of the camera
    private float _verticalRotation;

    // tracks the vertical velocity of the character
    private Vector3 _verticalVelocity;

    private Vector3 _moveDirection;

    #region Catch References
    private void Awake()
    {
        _charController ??= GetComponent<CharacterController>();
        _camera = Camera.main;
    }
    #endregion

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
        _rawMoveInput = InputManager.GetMoveInput().normalized * moveSpeed;

        // only move when theres a valid input
        if (_rawMoveInput.sqrMagnitude >= 0.0001f)
        {
            // create a direction based on orientation and input
            Vector3 targetDirection = transform.right * _rawMoveInput.x + transform.forward * _rawMoveInput.z;

            // smoothen the movement vector and apply
            _moveDirection = Vector3.Lerp(_moveDirection, targetDirection, Time.deltaTime * 10f);
            _charController.Move(_moveDirection * Time.deltaTime);
        }
    }

    private void HandleRotate()
    {
        _rawLookInput = InputManager.GetLookInput() * lookSpeed;

        // smoothout the look input for smooth camera rotation
        _smoothLookInput = Vector2.Lerp(_smoothLookInput, _rawLookInput, Time.deltaTime * 10.0f);

        _verticalRotation -= _smoothLookInput.y;

        // clamp the vertical rotation to avoid over rotation
        _verticalRotation = Mathf.Clamp(_verticalRotation, -lookLimit, lookLimit);

        _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * _smoothLookInput.x);
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

        _charController.Move(_verticalVelocity * Time.deltaTime);
    }

    public bool IsMoving()
    {
        // return true if the player is moving
        return _rawMoveInput.sqrMagnitude >= 0.0001f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 spherOrigin = checkSphere.position + Vector3.down * groundedOffset;

        Gizmos.DrawWireSphere(spherOrigin, radius);
    }
}