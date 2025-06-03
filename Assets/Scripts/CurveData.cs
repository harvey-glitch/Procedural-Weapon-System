using UnityEngine;

[CreateAssetMenu(fileName = "ProceduralCurve", menuName = "NewProceduralCurve/Curve")]
public class CurveData : ScriptableObject
{
    public AnimationCurve positionX = AnimationCurve.EaseInOut(0, 0, 1f, 0f);
    public AnimationCurve positionY = AnimationCurve.EaseInOut(0, 0, 1f, 0f);
    public AnimationCurve positionZ = AnimationCurve.EaseInOut(0, 0, 1f, 0f);

    public AnimationCurve rotationX = AnimationCurve.EaseInOut(0, 0, 1f, 0f);
    public AnimationCurve rotationY = AnimationCurve.EaseInOut(0, 0, 1f, 0f);
    public AnimationCurve rotationZ = AnimationCurve.EaseInOut(0, 0, 1f, 0f);

    public float duration;
}
