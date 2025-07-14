using UnityEngine;
using System.Collections.Generic;

public abstract class MotionOffset : MonoBehaviour
{
    [HideInInspector]
    public FirstPersonController player;

    [HideInInspector]
    public InputManager input;
    public struct SpringTransform
    {
        public Vector3 position;
        public Vector3 rotation;

        public SpringTransform(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private void Awake()
    {
        player ??= FindFirstObjectByType<FirstPersonController>();
        input ??= FindFirstObjectByType<InputManager>();
    }

    // abstract method to be implemented by deriving class
    public abstract SpringTransform GetOffset();

    public abstract void UpdateOffset();
}
