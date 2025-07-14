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

    [Space(2), Tooltip("How much the weapon tilt when moving sideways")]
    public float titlAmount = 5f;

    [Space(2), Tooltip("Extra multiplier to scale the overall effect")]
    public float multiplier = 2f;

    private float _elapsedTime;
    private float _speedFactor;

    public override SpringTransform GetOffset()
    {
        float xPos = Mathf.Sin(_elapsedTime) * xAmplitude * _speedFactor;
        float yPos = -Mathf.Abs(Mathf.Sin(_elapsedTime)) * yAmplitude * _speedFactor;

        float yRot = Mathf.Cos(_elapsedTime) * yRotation * _speedFactor;
        float zRot = 0f;

        if (player.IsMoving())
        {
            // check if the player is moving side way
            float dot = Vector3.Dot(player.moveVector.normalized, player.transform.right);

            if (dot > 0.5f)
                // moving to the right
                zRot = -titlAmount;
            else if (dot < -0.5f)
                // moving to the left
                zRot = titlAmount;
        }

        return new SpringTransform(
            new Vector3(xPos, yPos, 0f),
            new Vector3(0f, yRot, zRot));
    }

    public override void UpdateOffset()
    {
        if (player.IsMoving())
            _elapsedTime += Time.deltaTime * frequency;

        else
            _elapsedTime = 0f;


        _speedFactor = player.IsMovingForward() ? multiplier : 1f;

        SpringTransform offset = GetOffset();
        SpringSystem.instance.AddConstantForce("Bob", offset.position, offset.rotation);
    }
}
