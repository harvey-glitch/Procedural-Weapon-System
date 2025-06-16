using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Springs
{
    [Tooltip("String use to retrieve specific spring vector on the dictionary")]
    public string springID;

    [Space(5)]
    public SpringVector3 positionSpring = new SpringVector3();

    [Space(5)]
    public SpringVector3 rotationSpring = new SpringVector3();

    [Space(5), Tooltip("Flag to check where this spring should include rotation or not")]
    public bool includeRotation = false;
}

public class SpringSystem : MonoBehaviour
{
    public static SpringSystem instance;

    [Header("List of Springs Vectors")]
    public List<Springs> springsList = new();

    private Dictionary<string, Springs> springMap = new();

    [Header("Transform to Apply Spring Offsets")]
    public Transform targetTransform;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Awake()
    {
        // build a dictionary of springs for easy access
        foreach (var spring in springsList)
        {
            if (!springMap.ContainsKey(spring.springID))
                springMap.Add(spring.springID, spring);
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
        _originalPosition = targetTransform.localPosition;
        _originalRotation = targetTransform.localRotation;
    }

    private void Update()
    {
        // clear the total position and rotation at the start of update
        Vector3 totalPosition = Vector3.zero;
        Vector3 totalRotation = Vector3.zero;

        float dt = Time.deltaTime; // advance time

        foreach (var spring in springsList)
        {
            spring.positionSpring.Update(spring.positionSpring.Target, dt);
            totalPosition += spring.positionSpring.value * spring.positionSpring.weight;

            // only apply rotation for springs with includeRotation flag set to true
            if (spring.includeRotation)
            {
                spring.rotationSpring.Update(spring.rotationSpring.Target, dt);
                totalRotation += spring.rotationSpring.value * spring.rotationSpring.weight;
            }
        }

        if (targetTransform != null)
        {
            // apply the total positions and rotation on the target transform
            targetTransform.localPosition = _originalPosition + totalPosition;
            targetTransform.localRotation = _originalRotation * Quaternion.Euler(totalRotation);
            Debug.Log(totalRotation);
        }
    }

    // method use to add constant / continues force on the spring
    public void AddConstantForce(string id, Vector3 posTarget, Vector3 rotTarget)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.positionSpring.Target = posTarget;
            spring.rotationSpring.Target = rotTarget;
        }
    }

    // method use to apply an additive impulse force to the spring. Useful for stacking effects.
    public void AddImpulseForce(string id, Vector3 posForce, Vector3 rotForce)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.positionSpring.AddImpulseForce(posForce);
            spring.rotationSpring.AddImpulseForce(rotForce);
        }
    }

    // method use to apply a non-additive force to the spring. Useful for one-shot effect.
    public void SetInstantForce(string id, Vector3 posForce, Vector3 rotForce)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.positionSpring.SetImpulseForce(posForce);
            spring.rotationSpring.SetImpulseForce(rotForce);
        }
    }

    // helper method to get specific spring vector in the dictionary
    public SpringVector3 GetPositionSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.positionSpring : null;

    public SpringVector3 GetRotationSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.rotationSpring : null;

    // helper method to set the weight of specific spring position or rotation
    public void SetSpringWeight(string id, float positionWeight, float rotationWeight)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            if (spring.positionSpring != null)
                spring.positionSpring.weight = positionWeight;

            if (spring.rotationSpring != null)
                spring.rotationSpring.weight = rotationWeight;
        }
    }
}
