using UnityEngine;

public class WeaponIdle : MotionOffset
{
    [Header("Variables")]
    [Space(2), Tooltip("How fast the horizontal motion is")]
    public float xFrequency;

    [Space(2), Tooltip("Side-to-side strength of the motion")]
    public float xAmplitude;

    [Space(2), Tooltip("How fast the vertical motion is")]
    public float yFrequency;

    [Space(2), Tooltip("Up-and-down strength of the motion")]
    public float yAmplitude;

    private float _xTime;
    private float _yTime;

    public override Vector3 GetPositionOffset()
    {
        float x = (Mathf.PerlinNoise(_xTime, 0.5f) * 2f - 1f) * xAmplitude;
        float y = Mathf.Sin(_yTime + Mathf.PI / 2f) * yAmplitude;

        return new Vector3(x, y, 0f);
    }

    public override Vector3 GetRotationOffset()
    {
        return Vector3.zero;
    }

    public override void CustomMotionHandler()
    {
        if (!player.IsMoving())
        {
            _xTime += Time.deltaTime * xFrequency;
            _yTime += Time.deltaTime * yFrequency;
        }
        else
        {
            _xTime = 0f;
            _yTime = 0f;
        }

        SpringSystem.instance.AddConstantForce("Idle", GetPositionOffset(), GetRotationOffset());
    }
}
