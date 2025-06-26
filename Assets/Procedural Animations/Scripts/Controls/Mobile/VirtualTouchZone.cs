using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class VirtualTouchZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Controls how sensitive the look input is when swiping")]
    public float sensitivity = 0.1f;

    public Vector2 LookInput => _lookInput;

    private Vector2 _lookInput;
    private Vector2 _lastTouchPosition;
    private int _swipeFingerId = -1;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        _lookInput = Vector2.zero;

        foreach (var touch in Touch.activeTouches)
        {
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    if (_swipeFingerId == -1 && touch.screenPosition.x > Screen.width / 2f)
                    {
                        _swipeFingerId = touch.finger.index;
                        _lastTouchPosition = touch.screenPosition;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (touch.finger.index == _swipeFingerId)
                    {
                        _lookInput = (touch.screenPosition - _lastTouchPosition) * sensitivity;
                        _lastTouchPosition = touch.screenPosition;
                    }
                    break;

                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (touch.finger.index == _swipeFingerId)
                    {
                        _swipeFingerId = -1;
                    }
                    break;
            }
        }

        // Safety check: if finger is gone, clear ID
        if (_swipeFingerId != -1 && !FingerStillExists(_swipeFingerId))
        {
            _swipeFingerId = -1;
        }
    }

    private bool FingerStillExists(int id)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.finger.index == id)
                return true;
        }
        return false;
    }
}
