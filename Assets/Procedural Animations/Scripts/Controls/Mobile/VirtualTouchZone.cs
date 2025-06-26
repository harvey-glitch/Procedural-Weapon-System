using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class VirtualTouchZone : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Controls how sensitive the look input is when swiping")]
    public float sensitivity = 0.1f;

    // returns the current look input (read-only)
    public Vector2 LookInput => _lookInput;

    // current input value used for looking around
    private Vector2 _lookInput;

    // stores the last touch position
    private Vector2 _lastTouchPosition;

    // track the first finger to touch the right half of the screen, this finger will be use for rotation
    private int _swipeFingerId = -1;

    private void Update()
    {
        _lookInput = Vector2.zero;

        // loop through all active touches on the screen
        foreach (Touch touch in Input.touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // only assign a new finger for swiping if none is currently tracked
                    if (_swipeFingerId == -1 && touch.position.x > Screen.width / 2f)
                    {
                        // save the first finger on the right side for rotation
                        _swipeFingerId = touch.fingerId;
                        _lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Moved:
                    // if this is the swipe finger, calculate how far it moved
                    if (touch.fingerId == _swipeFingerId)
                    {
                        // calculate the difference between current and last touch position
                        _lookInput = (touch.position - _lastTouchPosition) * sensitivity;
                        _lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended: case TouchPhase.Canceled:
                    // if the swipe finger is lifted off, clear the tracking
                    if (touch.fingerId == _swipeFingerId)
                    {
                        _swipeFingerId = -1;
                    }
                    break;
            }
        }

        // safety check incase touch phase ended / canceled doesnt call due to bugs / glitches
        if (_swipeFingerId != -1 && !FingerStillExists(_swipeFingerId))
        {
            _swipeFingerId = -1;
        }
    }

    // helper function to double-check if a finger is still active on the screen
    private bool FingerStillExists(int id)
    {
        foreach (var touch in Input.touches)
        {
            if (touch.fingerId == id)
                return true;
        }

        return false;
    }
}