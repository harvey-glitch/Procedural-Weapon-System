using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Springs
{
    [Tooltip("String use to retrieve specific spring vector on the dictionary")]
    public string SpringID;

    [Space(5)]
    public SpringVector3 PositionSpring = new SpringVector3();

    [Space(5)]
    public SpringVector3 RotationSpring = new SpringVector3();

    [Space(5), Tooltip("Flag to check where this spring should include rotation or not")]
    public bool IncludeRotation = false;
}

public class SpringSystem : MonoBehaviour
{
    public static SpringSystem instance;

    [Header("List of Springs Vectors")]
    public List<Springs> SpringsList = new();

    private Dictionary<string, Springs> SpringMap = new();

    [Header("Transform to Apply Spring Offsets")]
    public Transform TargetTransform;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        // build a dictionary of springs for easy access
        foreach (var spring in SpringsList)
        {
            if (!SpringMap.ContainsKey(spring.SpringID))
                SpringMap.Add(spring.SpringID, spring);
        }

        #region Singleton Pattern
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // destroy duplicates
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
    }

    private void Start()
    {
        // store the original transform of target transform
        // useful for maintaining its original transfrom when animating
        _originalPosition = TargetTransform.localPosition;
        _originalRotation = TargetTransform.localRotation;
    }

    private void Update()
    {
        // clear the total position and rotation at the start of update
        Vector3 totalPosition = Vector3.zero;
        Vector3 totalRotation = Vector3.zero;

        float dt = Time.deltaTime; // advance time

        foreach (var spring in SpringsList)
        {
            spring.PositionSpring.Update(spring.PositionSpring.Target, dt);
            totalPosition += spring.PositionSpring.Value * spring.PositionSpring.Weight;

            // only apply rotation for springs with includeRotation flag set to true
            if (spring.IncludeRotation)
            {
                spring.RotationSpring.Update(spring.RotationSpring.Target, dt);
                totalRotation += spring.RotationSpring.Value * spring.RotationSpring.Weight;
            }
        }

        if (TargetTransform != null)
        {
            // apply the total positions and rotation on the target transform
            TargetTransform.localPosition = _originalPosition + totalPosition;
            TargetTransform.localRotation = _originalRotation * Quaternion.Euler(totalRotation);
        }
    }

    // method use to add constant / continues force on the spring
    public void AddConstantForce(string id, Vector3 posTarget, Vector3 rotTarget)
    {
        if (SpringMap.TryGetValue(id, out var spring))
        {
            spring.PositionSpring.Target = posTarget;
            spring.RotationSpring.Target = rotTarget;
        }
    }

    // method use to apply an additive impulse force to the spring. Useful for stacking effects.
    public void AddImpulseForce(string id, Vector3 posForce, Vector3 rotForce)
    {
        if (SpringMap.TryGetValue(id, out var spring))
        {
            spring.PositionSpring.AddImpulseForce(posForce);
            spring.RotationSpring.AddImpulseForce(rotForce);
        }
    }

    // helper method to get specific spring vector in the dictionary
    public SpringVector3 GetPositionSpring(string id) =>
        SpringMap.TryGetValue(id, out var spring) ? spring.PositionSpring : null;

    public SpringVector3 GetRotationSpring(string id) =>
        SpringMap.TryGetValue(id, out var spring) ? spring.RotationSpring : null;

    // helper method to set the weight of specific spring position or rotation
    public void SetSpringWeight(string id, float positionWeight, float rotationWeight)
    {
        if (SpringMap.TryGetValue(id, out var spring))
        {
            if (spring.PositionSpring != null)
                spring.PositionSpring.Weight = positionWeight;

            if (spring.RotationSpring != null)
                spring.RotationSpring.Weight = rotationWeight;
        }
    }
}
