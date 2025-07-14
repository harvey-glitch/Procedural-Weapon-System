using UnityEngine;

public class WeaponSway : MotionOffset
{
    [Header("Variables")]
    [Space(2), Tooltip("How responsive the motion is to input changes")]
    public float sensitivity = 0.02f;

    [Space(2), Tooltip("Maximum limit this motion can reach")]
    public float maxLimit = 0.1f;

    public override SpringTransform GetOffset()
    {
        Vector3 pos = new Vector3(-input.LookInput.x, -input.LookInput.y, 0f) * sensitivity;

        return new SpringTransform(
            Vector3.ClampMagnitude(pos, maxLimit),
            Vector3.zero);
    }

    public override void UpdateOffset()
    {
        SpringTransform offset = GetOffset();
        SpringSystem.instance.AddConstantForce("Sway", offset.position, offset.rotation);
    }
}
