using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [Tooltip("Reference to the whole joystick interface")]
    public RectTransform JoystickInterface;

    [Tooltip("Reference to the joystick's knob / handle")]
    public RectTransform JoystickHandle;

    [Header("Settings")]
    [Tooltip("Maximum distance the joystick handle can move from the center.")]
    public float Radius = 50f;

    [Tooltip("Minimum movement threshold to register input")]
    public float DeadZone = 0.1f;

    // returns the current movement input (read-only)
    public Vector2 MoveInput => _moveInput;

    // current input value used for moving around
    private Vector2 _moveInput;

    // flag to check wether the user is touching the screen or not
    private bool _isTouching;


    private void Start()
    {
        // disable the joystick at the start by fault
        SetJoystickVisibility(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Only allow touches on the left half of the screen
        if (eventData.position.x > Screen.width / 2f)
            return;

        // enable the virtual joystick
        SetJoystickVisibility(true);

        // set the joystick's position to touch position
        SetJoystickPosition(eventData.position);

        _isTouching = true; // mark as touching
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ignore when the user is not touching the screen
        if (!_isTouching)
            return;

        // convert touch position to local UI position relative to the joystick position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            JoystickInterface, eventData.position, null, out Vector2 localPoint);

        // if the touch is within the dead zone, ignore movement
        if (localPoint.magnitude < Radius * DeadZone)
            _moveInput = Vector2.zero;

        else
            // normalize the direction to keep it consistent in all directions
            _moveInput = localPoint.normalized;

        // move the joystick handle visually based on input
        SetAnchoredPosition(JoystickHandle, _moveInput * Radius);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _moveInput = Vector2.zero;
        _isTouching = false;
        SetJoystickVisibility(false);
    }

    #region Utility Methods
    private void SetJoystickPosition(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            JoystickInterface.parent as RectTransform, touchPosition, null, out Vector2 anchoredPosition);

        SetAnchoredPosition(JoystickInterface, anchoredPosition);
        SetAnchoredPosition(JoystickHandle, Vector2.zero);
    }

    private void SetAnchoredPosition(RectTransform rect, Vector2 position)
    {
        rect.anchoredPosition = position;
    }

    private void SetJoystickVisibility(bool state)
    {
        JoystickInterface.gameObject.SetActive(state);
    }
    #endregion
}
