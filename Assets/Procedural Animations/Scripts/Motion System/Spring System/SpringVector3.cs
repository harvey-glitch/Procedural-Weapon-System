using UnityEngine;

[System.Serializable]
public class SpringVector3
{
    [Tooltip("Controls how fast the spring returns to its target")]
    public float stiffness = 100f;

    [Range(0.1f, 1f), Tooltip("Controls how much the spring slows over time")]
    public float damping = 0.5f;

    [Range(0f, 1f), Tooltip("Controls how much influence this motion has")]
    public float weight = 1f;

    [HideInInspector] public Vector3 target = Vector3.zero;
    public Vector3 value { get; private set; }
    public Vector3 velocity { get; private set; }

    public void Update(Vector3 newTarget, float deltaTime)
    {
        target = newTarget;

        Vector3 displacement = value - target;
        Vector3 springForce = -stiffness * displacement;
        Vector3 dampingForce = -2f * damping * Mathf.Sqrt(stiffness) * velocity;

        Vector3 acceleration = springForce + dampingForce;

        velocity += acceleration * deltaTime;
        value += velocity * deltaTime;
    }

    public void AddImpulseForce(Vector3 force)
    {
        velocity += force;
    }

    public bool IsAtRest(float threshold = 0.0001f, float velocityThreshold = 0.001f)
    {
        return (target - value).sqrMagnitude < threshold * threshold &&
               velocity.sqrMagnitude < velocityThreshold * velocityThreshold;
    }
}
