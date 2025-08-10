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
}

public class SpringSystem : MonoBehaviour
{
    public static SpringSystem instance;

    [Header("Spring Configuration")]
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

        float elapsedTime = Time.deltaTime;

        foreach (var spring in springsList)
        {
            spring.PositionSpring.Update(spring.PositionSpring.target, elapsedTime);
            totalPosition += spring.PositionSpring.value * spring.PositionSpring.weight;

            spring.RotationSpring.Update(spring.RotationSpring.target, elapsedTime);
            totalRotation += spring.RotationSpring.value * spring.RotationSpring.weight;
        }

        if (targetTransform != null)
        {
            // apply the total positions and rotation on the target transform
            targetTransform.localPosition = _originalPosition + totalPosition;
            targetTransform.localRotation = _originalRotation * Quaternion.Euler(totalRotation);
        }
    }

    #region Utility Methods
    public void AddConstantForce(string springId, Vector3 posTarget, Vector3 rotTarget, params string[] blockingIds)
    {
        float weight = IsAnyMotionActive(blockingIds) ? 0.3f : 1f;
        SetSpringWeight(springId, weight);

        if (springMap.TryGetValue(springId, out var spring))
        {
            spring.PositionSpring.target = posTarget;
            spring.RotationSpring.target = rotTarget;
        }
    }

    // method use to apply an additive impulse force to the spring. Useful for stacking effects.
    public void AddImpulseForce(string springId, Vector3 posForce, Vector3 rotForce, params string[] blockingIds)
    {
        if (IsAnyMotionActive(blockingIds))
            return;

        if (springMap.TryGetValue(springId, out var spring))
        {
            spring.PositionSpring.AddImpulseForce(posForce);
            spring.RotationSpring.AddImpulseForce(rotForce);
        }
    }

    // helper method to stop specific motion when one or more motion is current playing
    public bool IsAnyMotionActive(params string[] springIds)
    {
        foreach (var activeSprings in springIds)
        {
            if (springMap.TryGetValue(activeSprings, out var blockingSprings))
            {
                // if this motion is active, return true
                if (!blockingSprings.PositionSpring.IsAtRest())
                    return true;
            }
        }
        // else return false
        return false;
    }

    // helper method to set the weight of specific spring position or rotation
    public void SetSpringWeight(string id, float weight)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            if (spring.PositionSpring != null)
            {
                float currentWeight = spring.PositionSpring.weight;
                currentWeight = Mathf.Lerp(currentWeight, weight, Time.deltaTime * 5f);
                spring.PositionSpring.weight = currentWeight;
            }

            if (spring.RotationSpring != null)
            {
                float currentWeight = spring.RotationSpring.weight;
                currentWeight = Mathf.Lerp(currentWeight, weight, Time.deltaTime * 5f);
                spring.RotationSpring.weight = currentWeight;
            }
        }
    }

    #endregion
}
