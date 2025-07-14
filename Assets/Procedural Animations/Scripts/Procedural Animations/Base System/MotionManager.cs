using UnityEngine;
using System.Collections.Generic;

public class MotionManager: MonoBehaviour
{
    [Tooltip("List of motion modules to process manually")]
    public List<MotionOffset> motionOffsets = new();

    private void Update()
    {
        foreach (var motion in motionOffsets)
        {
            if (motion == null) continue;

            motion.UpdateOffset();
        }
    }
}
