using UnityEngine;

public class WeaponBob : MotionOffset
{
    [Header("Variables")]

    [Space(2), Tooltip("How fast the motion is")]
    public float frequency = 8f;

    [Space(2), Tooltip("Side-to-side strength of the motion")]
    public float xAmplitude = 0.01f;

    [Space(2), Tooltip("Up-and-down strength of the motion")]
    public float yAmplitude = 0.02f;

    [Space(2), Tooltip("Side-to-side rotation strength of the motion")]
    public float yRotation = 2f;

    [Space(2), Tooltip("Extra multiplier to scale the overall effect")]
    public float multiplier = 2f;

    private float _elapsedTime;
    private float _speedFactor;

    public override Vector3 GetPositionOffset()
    {
        float x = Mathf.Sin(_elapsedTime) * xAmplitude * _speedFactor;
        float y = -Mathf.Abs(Mathf.Sin(_elapsedTime)) * yAmplitude * _speedFactor;

        return new Vector3(x, y, 0f);
    }

    public override Vector3 GetRotationOffset()
    {
        float y = Mathf.Cos(_elapsedTime) * yRotation * _speedFactor;

        return new Vector3(0f, y, 0f);
    }

    public override void CustomMotionHandler()
    {
        if (player.IsMoving())
            _elapsedTime += Time.deltaTime * frequency;

        else
            _elapsedTime = 0f;

        _speedFactor = player.IsMovingForward() ? multiplier : 1f;

        SpringSystem.instance.AddConstantForce("Bob", GetPositionOffset(), GetRotationOffset());
    }
}
