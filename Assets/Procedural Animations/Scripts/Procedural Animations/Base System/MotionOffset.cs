using UnityEngine;

public abstract class MotionOffset : MonoBehaviour
{
    [HideInInspector]
    public FirstPersonController player;

    [HideInInspector]
    public InputManager input;

    // read only variable for easy access
    public Vector3 positionOffset => GetPositionOffset();
    public Vector3 rotationOffset => GetRotationOffset();


    private void Awake()
    {
        player ??= FindFirstObjectByType<FirstPersonController>();
        input ??= FindFirstObjectByType<InputManager>();
    }

    // abstract method to be implemented by deriving class

    public abstract Vector3 GetPositionOffset();

    public abstract Vector3 GetRotationOffset();

    public abstract void CustomMotionHandler();
}
