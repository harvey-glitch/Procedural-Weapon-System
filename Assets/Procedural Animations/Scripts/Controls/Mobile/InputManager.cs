using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode{ Mobile, Standalone }
public class InputManager : MonoBehaviour
{
    [Header("Input")]
    public InputMode Mode;

    [Header("Mobile")]
    [Tooltip("Reference to the virtual joystick for movement")]
    public VirtualJoystick joystickInput;

    [Tooltip("Reference to the virtual touch zone for rotation")]
    public VirtualTouchZone touchZoneInput;

    public Canvas MobileInputCanvas;

    [Header("Standalone")]
    [Tooltip("Reference to the standalone input")]
    public PlayerInput standaloneInput;

    // public accessors if other scripts need them
    public Vector2 MoveInput => _moveInput;
    public Vector2 LookInput => _lookInput;
    public bool AttackInput => _attackInput;

    // vectors to store inputs
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _attackInput;

    void Awake()
    {
    #if UNITY_EDITOR
            // Keep the mode from Inspector for manual testing
            Debug.Log($"[InputManager] Running in editor, using manually set mode: {Mode}");
    #else
        // Auto-detect only on real device builds
    #if UNITY_ANDROID || UNITY_IOS
            Mode = InputMode.Mobile;
    #else
            Mode = InputMode.Standalone;
    #endif
    #endif
    }

    private void Start()
    {
        if (Mode == InputMode.Mobile)
            MobileInputCanvas.gameObject.SetActive(true);

        else
            MobileInputCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mode == InputMode.Mobile)
            UseMobileInputs();

        else
            UseStandaloneInputs();
    }

    private void UseMobileInputs()
    {
        _moveInput = joystickInput.MoveInput;
        _lookInput = touchZoneInput.LookInput;
    }

    private void UseStandaloneInputs()
    {
        _moveInput = standaloneInput.actions["Move"].ReadValue<Vector2>();
        _lookInput = standaloneInput.actions["Look"].ReadValue<Vector2>() * Time.deltaTime;
        _attackInput = standaloneInput.actions["Attack"].IsPressed();
    }
}
