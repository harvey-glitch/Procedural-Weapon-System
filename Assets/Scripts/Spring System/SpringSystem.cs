using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Springs
{
    public string springID;
    public SpringVector3 positionSpring = new SpringVector3();

    [Header("Add Rotation")]
    public bool includeRotaton = false;
    public SpringVector3 rotationSpring = new SpringVector3();
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

        float dt = Time.deltaTime;

        foreach (var spring in springsList)
        {
            spring.positionSpring.Update(spring.positionSpring.Target, dt);
            totalPosition += spring.positionSpring.Value;

            if (spring.includeRotaton)
            {
                spring.rotationSpring.Update(spring.rotationSpring.Target, dt);
                totalRotation += spring.rotationSpring.Value;
            }
        }

        if (targetTransform != null)
        {
            targetTransform.localPosition = _originalPosition + totalPosition;
            targetTransform.localRotation = _originalRotation * Quaternion.Euler(totalRotation);
        }
    }

    public void AddConstantForce(string id, Vector3 posTarget, Vector3 rotTarget)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.positionSpring.Target = posTarget;
            spring.rotationSpring.Target = rotTarget;
        }
    }

    public void AddImpulseForce(string id, Vector3 posForce, Vector3 rotForce)
    {
        if (springMap.TryGetValue(id, out var spring))
        {
            spring.positionSpring.AddImpulseForce(posForce);
            spring.rotationSpring.AddImpulseForce(rotForce);
        }
    }

    public SpringVector3 GetPositionSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.positionSpring : null;

    public SpringVector3 GetRotationSpring(string id) =>
        springMap.TryGetValue(id, out var spring) ? spring.rotationSpring : null;
}
