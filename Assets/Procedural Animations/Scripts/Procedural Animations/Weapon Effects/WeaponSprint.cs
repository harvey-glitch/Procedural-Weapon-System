using UnityEngine;

public class WeaponSprint : MotionOffset
{
    [Header("Variables")]
    [Space(2), Tooltip("Position of the weapon when sprinting")]
    public Vector3 position;

    [Space(2), Tooltip("Rotation of the weapon when sprinting")]
    public Vector3 rotation;

    public override Vector3 GetPositionOffset()
    {
        return position;
    }

    public override Vector3 GetRotationOffset()
    {
        return rotation;
    }

    public override void CustomMotionHandler()
    {
        Vector3 pos = Vector3.zero;
        Vector3 rot = Vector3.zero;

        if (player.IsMovingForward())
        {
            pos = GetPositionOffset();
            rot = GetRotationOffset();
        }

        SpringSystem.instance.AddConstantForce("Sprint", pos, rot);
    }
}
