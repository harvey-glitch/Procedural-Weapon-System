using UnityEngine;

public class WeaponSprint : MotionOffset
{
    [Header("Variables")]
    [Space(2), Tooltip("Position of the weapon when sprinting")]
    public Vector3 position;

    [Space(2), Tooltip("Rotation of the weapon when sprinting")]
    public Vector3 rotation;

    public override SpringTransform GetOffset()
    {
        return new SpringTransform(
            position,
            rotation);
    }
    public override void UpdateOffset()
    {
        Vector3 pos = Vector3.zero;
        Vector3 rot = Vector3.zero;

        if (player.IsMovingForward())
        {
            SpringTransform offset = GetOffset();
            pos = offset.position;
            rot = offset.rotation;
        }

        SpringSystem.instance.AddConstantForce("Sprint", pos, rot);
    }
}
