using UnityEngine;

public class WeaponRecoil : MotionOffset
{
    [Header("Varriables")]
    [Space(2), Tooltip("How much backward motion is applied")]
    public float kickback;

    [Space(2), Tooltip("Maximum amount this motion can reach")]
    public float maxKickback;

    [Space(2), Tooltip("How frequently the motion event can occur")]
    public float firerate;

    [Space(2), Tooltip("Rotation to apply when the motion is triggered")]
    public Vector3 rotation;

    private bool _isFiring;
    private float _nextFireTime;

    public override Vector3 GetPositionOffset()
    {
        float clampedZ = Mathf.Clamp(kickback, 0f, -maxKickback);
        return new Vector3(0f, 0f, clampedZ);
    }

    public override Vector3 GetRotationOffset()
    {
        return rotation;
    }

    public override void CustomMotionHandler()
    {
        // Only update _isFiring from AttackInput in Standalone mode
        if (input.Mode == InputMode.Standalone)
            _isFiring = input.AttackInput;

        if (_isFiring && Time.time >= _nextFireTime)
        {
            SpringSystem.instance.AddImpulseForce("Recoil", GetPositionOffset(), GetRotationOffset());
            _nextFireTime = Time.time + (1f / firerate);
        }
    }

    // can be call to manually set the firing value
    public void SetFiringState(bool newState)
    {
        _isFiring = newState;
    }
}
