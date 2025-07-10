using UnityEngine;

public class WeaponSway : MotionOffset
{
    [Header("Variables")]
    [Space(2), Tooltip("How responsive the motion is to input changes")]
    public float sensitivity = 0.02f;

    [Space(2), Tooltip("Maximum limit this motion can reach")]
    public float maxLimit = 0.1f;

    public override Vector3 GetPositionOffset()
    {
        Vector3 position = new Vector3(
            -input.LookInput.x, -input.LookInput.y, 0f) * sensitivity;

        // return a clamped vector
        return Vector3.ClampMagnitude(position, maxLimit);
    }

    public override Vector3 GetRotationOffset()
    {
        return Vector3.zero;
    }

    public override void CustomMotionHandler()
    {
        SpringSystem.instance.AddConstantForce("Sway", GetPositionOffset(), GetRotationOffset());
    }
}
