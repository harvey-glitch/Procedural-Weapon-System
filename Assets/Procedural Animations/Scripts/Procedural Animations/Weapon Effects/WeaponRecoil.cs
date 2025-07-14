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

    private float _nextFireTime;

    public override SpringTransform GetOffset()
    {
        // clapmed the recoil kickback
        float zPos = Mathf.Clamp(kickback, 0f, -maxKickback);

        return new SpringTransform(
            new Vector3(-0f, -0f, zPos),
            Vector3.zero);
    }

    public override void UpdateOffset()
    {
        if (input.AttackInput && Time.time >= _nextFireTime)
        {
            SpringTransform offset = GetOffset();
            SpringSystem.instance.AddImpulseForce("Recoil", offset.position, offset.rotation);
            _nextFireTime = Time.time + (1f / firerate);
        }
    }
}
