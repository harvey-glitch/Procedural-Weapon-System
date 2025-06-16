using UnityEngine;

[System.Serializable]
public class SpringVector3
{
    [Tooltip("How tightly the spring snaps to the target. Higher = faster response.")]
    public float stiffness = 100f;

    [Range(0.1f, 1f), Tooltip("How much the spring resists velocity. Lower = more overshoot, higher = tighter.")]
    public float damping = 0.5f;

    [Range(0f, 1f), Tooltip("Determine how much the influence of this motion")]
    public float weight = 1f;

    [HideInInspector] public Vector3 Target = Vector3.zero;
    public Vector3 value { get; private set; }
    public Vector3 velocity { get; private set; }

    public void Update(Vector3 target, float deltaTime)
    {
        Target = target;

        Vector3 displacement = value - Target;
        Vector3 springForce = -stiffness * displacement;
        Vector3 dampingForce = -2f * damping * Mathf.Sqrt(stiffness) * velocity;

        Vector3 acceleration = springForce + dampingForce;

        velocity += acceleration * deltaTime;
        value += velocity * deltaTime;
    }

    // method for accumulative force
    public void AddImpulseForce(Vector3 force)
    {
        velocity += force;
    }

    // method for non-accumulative force
    public void SetImpulseForce(Vector3 force)
    {
        value = force;
        velocity = Vector3.zero;
    }
}
