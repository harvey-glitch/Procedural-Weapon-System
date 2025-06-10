using UnityEngine;

public class SpringSway : MonoBehaviour
{
    [Header("Spring Settings")]
    public SpringVector3 positionSpring = new SpringVector3
    {
        Stiffness = 150f,
        Damping = 20f
    };

    [Header("Sway Settings")]
    public float swayAmount = 0.05f;  // How much mouse movement affects sway
    public float maxSwayOffset = 0.1f;

    private Vector3 initialLocalPos;

    void Start()
    {
        initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        Vector2 mouseDelta = GetMousePosition();

        // Convert mouse delta into sway force (invert X for natural feel)
        Vector3 swayForce = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f) * swayAmount;

        // Add force to spring
        positionSpring.AddImpulseForce(swayForce);

        positionSpring.Update(Vector3.zero, Time.deltaTime);

        // Clamp offset for max sway limit
        Vector3 clampedOffset = Vector3.ClampMagnitude(positionSpring.Value, maxSwayOffset);

        // Apply local position sway relative to initial position
        transform.localPosition = initialLocalPos + clampedOffset;
    }

    private Vector2 GetMousePosition()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
