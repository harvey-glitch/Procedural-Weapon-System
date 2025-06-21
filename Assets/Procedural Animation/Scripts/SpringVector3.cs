using UnityEngine;

[System.Serializable]
public class SpringVector3
{
    [Tooltip("Controls how fast the spring returns to its target")]
    public float Stiffness = 100f;

    [Range(0.1f, 1f), Tooltip("Controls how much the spring slowed overtime")]
    public float Damping = 0.5f;

    [Range(0f, 1f), Tooltip("Controls how much the influence of this motion")]
    public float Weight = 1f;

    [HideInInspector] public Vector3 Target = Vector3.zero;
    public Vector3 Value { get; private set; }
    public Vector3 Velocity { get; private set; }

    public void Update(Vector3 target, float deltaTime)
    {
        Target = target;

        Vector3 displacement = Value - Target;
        Vector3 springForce = -Stiffness * displacement;
        Vector3 dampingForce = -2f * Damping * Mathf.Sqrt(Stiffness) * Velocity;

        Vector3 acceleration = springForce + dampingForce;

        Velocity += acceleration * deltaTime;
        Value += Velocity * deltaTime;
    }

    // method to add instance velocity on the spring
    public void AddImpulseForce(Vector3 force)
    {
        Velocity += force;
    }

    // method for checking if the spring is already at rest (settled)
    public bool IsAtRest(float threshold = 0.0001f, float velocityThreshold = 0.001f)
    {
        return (Target - Value).sqrMagnitude < threshold * threshold &&
               Velocity.sqrMagnitude < velocityThreshold * velocityThreshold;
    }
}
