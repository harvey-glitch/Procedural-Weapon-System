using UnityEngine;

public class SpringController : MonoBehaviour
{
    [Header("RECOIL")]
    public float zPosition;
    public float xRotation;
    public float yRotation;
    public float firerate;

    private float _nextFireTime;

    [Header("BOBBING")]
    public float frequency;
    public float amplitude;
    public float zRotation;

    private float bobbingTimer;

    [Header("SWAYING")]
    public float swayAmount;
    public float maxSwayOffset;

    private void Update()
    {
        AddRecoil();
        AddBobbing();
        AddSwaying();
    }

    private void AddRecoil()
    {
        if (Input.GetMouseButton(0) && Time.time > _nextFireTime)
        {
            Vector3 kickback = new Vector3(0f, 0f, -zPosition);
            Vector3 rotation = new Vector3(xRotation, Random.Range(-yRotation, yRotation), 0f);
            SpringSystem.instance.AddImpulseForce("Recoiling", kickback, rotation);

            _nextFireTime = Time.time + (1f / firerate);
        }
    }

    private void AddBobbing()
    {
        Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float deltaTime = Time.deltaTime;

        // Advance time if moving
        if (velocity.sqrMagnitude >= 0.0001f)
            bobbingTimer += deltaTime * frequency;
        else
            bobbingTimer = 0f;

        // Generate bobbing offset
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;

        if (velocity.sqrMagnitude >= 0.0001f)
        {
            positionOffset.x = Mathf.Sin(bobbingTimer) * amplitude;
            positionOffset.y = Mathf.Abs(Mathf.Sin(bobbingTimer)) * amplitude;

            rotationOffset.z = Mathf.Sin(bobbingTimer * 2f) * -zRotation;
        }

        // Apply spring logic
        SpringSystem.instance.AddConstantForce("Bobbing", positionOffset, rotationOffset);
    }

    private void AddSwaying()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Convert mouse delta into sway force (invert X for natural feel)
        Vector3 swayForce = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f) * swayAmount;

        // Clamp offset for max sway limit
        Vector3 clampedSway = Vector3.ClampMagnitude(swayForce, maxSwayOffset);

        // Add force to spring
        SpringSystem.instance.AddImpulseForce("Swaying", clampedSway, Vector3.zero);
    }
}
