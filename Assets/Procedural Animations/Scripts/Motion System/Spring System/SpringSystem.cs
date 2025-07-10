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
            if (!springMap.ContainsKey(spring.SpringID))
                springMap.Add(spring.SpringID, spring);
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
        // if the spring list is empty of the target transform is null, terminate
        if (springsList == null || springsList.Count == 0 || targetTransform == null)
        {
            Debug.LogWarning("Either spring list is empty of the target transform is not set");
            return;
        }

        // clear the total position and rotation at the start of update
        Vector3 totalPosition = Vector3.zero;
        Vector3 totalRotation = Vector3.zero;

        bool isAnyEffectActive = false;
        float elapsedTime = Time.deltaTime;

        foreach (var spring in springsList)
        {
            if (!spring.PositionSpring.IsAtRest())
            {
                spring.PositionSpring.Update(spring.PositionSpring.target, elapsedTime);
                totalPosition += spring.PositionSpring.value * spring.PositionSpring.weight;
                isAnyEffectActive = true;
            }

            // only apply rotation for springs with includeRotation flag set to true
            if (spring.IncludeRotation)
            {
                spring.RotationSpring.Update(spring.RotationSpring.target, elapsedTime);
                totalRotation += spring.RotationSpring.value * spring.RotationSpring.weight;
            }
        }

        if (targetTransform != null && isAnyEffectActive)
        {
            // apply the total positions and rotation on the target transform
            targetTransform.localPosition = _originalPosition + totalPosition;
            targetTransform.localRotation = _originalRotation * Quaternion.Euler(totalRotation);
        }
    }

    // method use to add constant / continues force on the spring
    public void AddConstantForce(string id, Vector3 posTarget, Vector3 rotTarget)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.PositionSpring.target = posTarget;
            spring.RotationSpring.target = rotTarget;
        }
    }

    // method use to apply an additive impulse force to the spring. Useful for stacking effects.
    public void AddImpulseForce(string id, Vector3 posForce, Vector3 rotForce)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.PositionSpring.AddImpulseForce(posForce);
            spring.RotationSpring.AddImpulseForce(rotForce);
        }
    }

    // helper method to get specific spring vector in the dictionary
    public SpringVector3 GetPositionSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.PositionSpring : null;

    public SpringVector3 GetRotationSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.RotationSpring : null;

    // helper method to set the weight of specific spring position or rotation
    public void SetSpringWeight(string id, float positionWeight, float rotationWeight)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            if (spring.PositionSpring != null)
                spring.PositionSpring.weight = positionWeight;

            if (spring.RotationSpring != null)
                spring.RotationSpring.weight = rotationWeight;
        }
    }
}
