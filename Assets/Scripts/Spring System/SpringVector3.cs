using UnityEngine;

[System.Serializable]
public class SpringVector3
{
    public Vector3 Value { get; private set; }
    public Vector3 Velocity { get; private set; }

    [Header("SPRING")]
    [Tooltip("How quickly the motion slows down, higher value means faster decay.")]
    public float Damping = 20f;

    [Tooltip("How tightly the spring snaps to the target, higher value means stiffer spring.")]
    public float Stiffness = 150f;

    [System.NonSerialized]
    public Vector3 Target = Vector3.zero;

    public void Update(Vector3 target, float deltaTime)
    {
        Vector3 force = (target - Value) * Stiffness;
        Velocity += force * deltaTime;
        Velocity *= Mathf.Exp(-Damping * deltaTime);
        Value += Velocity * deltaTime;
    }

    public void AddImpulseForce(Vector3 force)
    {
        Velocity += force;
    }
}
