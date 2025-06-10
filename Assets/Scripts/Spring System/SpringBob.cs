using UnityEngine;
public class SpringBob : MonoBehaviour
{
    public SpringVector3 positionSpring = new SpringVector3();
    public SpringVector3 rotationSpring = new SpringVector3();

    public Transform targetTransform;

    public float frequency = 1.5f; // Speed of bob
    public float amplitude = 0.05f;   // Amount of movement
    public float zAmplitude;

    private float bobTimer = 0f;

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Advance time if moving
        if (IsMoving())
            bobTimer += deltaTime * frequency;
        else
            bobTimer = 0f;

        // Generate bobbing offset
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotationOffset = Vector3.zero;
        if (IsMoving())
        {
            positionOffset.x = Mathf.Sin(bobTimer) * amplitude;
            positionOffset.y = Mathf.Abs(Mathf.Sin(bobTimer)) * amplitude;

            rotationOffset.z = Mathf.Sin(bobTimer * 2f) * zAmplitude;
            //targetOffset.x = Mathf.Sin(bobTimer * 2f) * bobAmount;
            //targetOffset.y = Mathf.Cos(bobTimer * 4f) * bobAmount;
        }

        // Apply spring logic
        positionSpring.Update(positionOffset, deltaTime);
        rotationSpring.Update(rotationOffset, deltaTime);

        // Apply result to transform (local position recommended)
        targetTransform.localPosition = positionSpring.Value;
        targetTransform.localRotation = Quaternion.Euler(rotationSpring.Value);
    }

    bool IsMoving()
    {
        // Replace this with your actual player movement check
        return Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0;
    }
}
