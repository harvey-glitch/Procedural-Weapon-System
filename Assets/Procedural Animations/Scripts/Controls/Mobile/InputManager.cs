using UnityEngine;
using UnityEngine.InputSystem;

public enum InputMode{ Mobile, Standalone }
public class InputManager : MonoBehaviour
{
    [Header("Input")]
    public InputMode Mode;

    [Header("Mobile")]
    [Tooltip("Reference to the virtual joystick for movement")]
    public VirtualJoystick Joystick;

    [Tooltip("Reference to the virtual touch zone for rotation")]
    public VirtualTouchZone TouchZone;

    public Canvas MobileInputCanvas;
    [Header("Standalone")]
    [Tooltip("Reference to the standalone input")]
    public PlayerInput StandaloneInput;

    // public accessors if other scripts need them
    public Vector2 MoveInput => _moveInput;
    public Vector2 LookInput => _lookInput;

    // vectors to store inputs
    private Vector2 _moveInput;
    private Vector2 _lookInput;

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
        {
            // read input from mobile controls (joystick and touch)
            _moveInput = Joystick.MoveInput;
            _lookInput = TouchZone.LookInput;
        }
        else
        {
            // read input from new input system (keyboard and mouse)
            _moveInput = StandaloneInput.actions["Move"].ReadValue<Vector2>();
            _lookInput = StandaloneInput.actions["Look"].ReadValue<Vector2>();
            _lookInput *= Time.deltaTime;
        }
    }
}
