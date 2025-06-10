using UnityEngine;

public class SpringRecoil : MonoBehaviour
{
    [Header("Spring Settings")]
    public SpringVector3 positionSpring = new SpringVector3
    {
        Stiffness = 150f,
        Damping = 20f
    };

    public SpringVector3 rotationSpring = new SpringVector3
    {
        Stiffness = 150f,
        Damping = 20f
    };

    [Header("RECOIL POSITION")]
    public float zPosition;

    [Header("RECOIL ROTATION")]
    public float xRotation;
    public float yRotation;
    public float firerate;

    [Header("Recoil Limits")]
    public float maxPositionKickback = -0.2f;
    public float maxRotationKickback = 10f; // degrees

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private float _nextFireTime;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time > _nextFireTime)
        {
            AddRecoil(new Vector3(0f, 0f, -zPosition), new Vector3(-xRotation, Random.Range(-yRotation, yRotation), 0f));
            _nextFireTime = Time.time + (1f / firerate);
        }

        // Update springs toward rest (zero)
        positionSpring.Update(Vector3.zero, Time.deltaTime);
        rotationSpring.Update(Vector3.zero, Time.deltaTime);

        // Clamp recoil offsets
        Vector3 posOffset = Vector3.ClampMagnitude(positionSpring.Value, maxPositionKickback);
        Vector3 rotOffset = Vector3.ClampMagnitude(rotationSpring.Value, maxRotationKickback);

        // Apply recoil (local position + local rotation)
        transform.localPosition = initialLocalPos + posOffset;
        transform.localRotation = initialLocalRot * Quaternion.Euler(rotOffset);
    }

    /// <summary>
    /// Add recoil impulses for position and rotation
    /// </summary>
    public void AddRecoil(Vector3 posKick, Vector3 rotKick)
    {
        positionSpring.AddImpulseForce(posKick);
        rotationSpring.AddImpulseForce(rotKick);
    }
}
