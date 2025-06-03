using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAnimator : MonoBehaviour
{
    public static WeaponAnimator instance;

    [System.Serializable]
    public class Motions
    {
        [Tooltip("Animation curves used to animate weapon's position and rotation")]
        public CurveData curveData;

        [Tooltip("String used to identify the motion")]
        public string motionID;

        [Range(0, 1)]
        [Tooltip("Determine the intensity of the motion")]
        public float weight = 1f;

        [HideInInspector] public Vector3 positionOffset;
        [HideInInspector] public Vector3 rotationOffset;
        [HideInInspector] public Coroutine coroutine;
        [HideInInspector] public bool isActive;
        [HideInInspector] public float elapsedTime;
    }

    [Header("MOTIONS")]
    [Tooltip("List of all motions for animations, *recoil / bob etc")]
    public List<Motions> motionList = new List<Motions>();

    [Header("WEAPON")]
    [Tooltip("transform reference of the weapon root to animate")]
    public Transform weaponRoot;

    private Dictionary<string, Motions> motionDictionary;

    private Vector3 _originalWeaponPos;
    private Quaternion _originalWeaponRot;

    private void Awake()
    {
        // build dictionary for quick access
        motionDictionary = new Dictionary<string, Motions>();
        foreach (var motion in motionList)
        {
            if (!motionDictionary.ContainsKey(motion.motionID))
                motionDictionary.Add(motion.motionID, motion);
        }

        #region Singleton Pattern
        // singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start()
    {
        // store the original transform of weapon root
        _originalWeaponPos = weaponRoot.localPosition;
        _originalWeaponRot = weaponRoot.localRotation;
    }

    private void Update()
    {
        ApplyMotionOffsets();
    }

    public void ApplyMotionOffsets()
    {
        Vector3 totalPosition = Vector3.zero;
        Vector3 totalRotation = Vector3.zero;

        // looped through all motions in the list
        foreach (var motion in motionList)
        {
            // automatically reset the remaining offsets in all inactive motions
            if (!motion.isActive)
            {
                // only interpolates if the offsets are not equal to zero to avoid unncessary calculation
                if ((motion.positionOffset + motion.rotationOffset).sqrMagnitude >= 0.0001f)
                {
                    motion.positionOffset = Vector3.Lerp(
                        motion.positionOffset, Vector3.zero, Time.deltaTime * 10f);

                    motion.rotationOffset = Vector3.Lerp(
                        motion.rotationOffset, Vector3.zero, Time.deltaTime * 10f);
                }
            }

            // sum up all position and rotation from active motions
            totalPosition += motion.positionOffset * motion.weight;
            totalRotation += motion.rotationOffset * motion.weight;
        }

        // apply the total offsets to the weapon root
        weaponRoot.localPosition = _originalWeaponPos + totalPosition;
        weaponRoot.localRotation = _originalWeaponRot * Quaternion.Euler(totalRotation);
    }

    public void PlayMotionLoop(string motionToPlay, bool motionState, params string[] blockingMotions)
    {
        Motions motion = GetMotion(motionToPlay);

        // prevents the motion from playing if any blocking motion are active
        if (IsAnyMotionBlocking(blockingMotions))
        {
            motion.elapsedTime = 0f; // reset time to avoid sudden jumps when resumed
            motion.isActive = false;
            return; // exit immediately so the rest of the code doesn't get executed
        }

        // only update the motion's active state if it's changed
        if (motion.isActive != motionState)
        {
            motion.isActive = motionState;

            if (!motion.isActive)
            {
                motion.elapsedTime = 0f;
            }
        }

        // if the motion is valid and active, evaluate and apply its offsets
        if (motion != null && motion.isActive)
        {
            motion.elapsedTime += Time.deltaTime;

            // normalize the time over the motion's duration for looping
            float duration = motion.curveData.duration;
            float progress = Mathf.Repeat(motion.elapsedTime, duration) / duration;

            // apply motion offsets using the evaluated curve value
            EvaluateCurve(motion, progress);
        }
    }

    public void PlayMotionOnce(string motionID, params string[] blockingMotions)
    {
        Motions motion = GetMotion(motionID);

        // prevents the motion from playing if any blocking motion are active
        if (IsAnyMotionBlocking(blockingMotions))
        {
            return;
        }

        if (motion != null)
        {
            // if the motion is currently playing, stop it first
            if (motion.coroutine != null)
            {
                StopCoroutine(motion.coroutine);
            }

            motion.coroutine = StartCoroutine(StartMotion(motion));
        }
    }

    public bool IsAnyMotionBlocking(params string[] blockingMotions)
    {
        // loop through blocking motions
        foreach (string motionIDs in blockingMotions)
        {
            // store each one ids for reference
            var blockerMotion = GetMotion(motionIDs);

            // stop the current motion if any of motions defined in parameter are playing
            if (blockerMotion != null && blockerMotion.isActive)
            {
                return true;
            }
        }
        return false;
    }

    public Motions GetMotion(string motionID)
    {
        if (motionDictionary.TryGetValue(motionID, out Motions motion))
        {
            return motion;
        }
        else
        {
            Debug.LogWarning("Motion with ID" + " " + motionID + " "  + "not found!");
            return null;
        }
    }

    private IEnumerator StartMotion(Motions motion)
    {
        // mark the motion as active
        motion.isActive = true;
        motion.elapsedTime = 0;
        float duration = motion.curveData.duration;

        // run the motion over time until the duration is reached
        while (motion.elapsedTime < duration)
        {
            motion.elapsedTime += Time.deltaTime;

            // normalized the motion time based on duration
            float progress = Mathf.Clamp01(motion.elapsedTime / duration);

            // evaluate motion offsets based on normalized t
            EvaluateCurve(motion, progress);

            yield return null; // wait until next frame
        }

        // Mark the motion as inactive
        motion.isActive = false;
    }

    // method for evaluating the curve based on parameter  "t"
    private void EvaluateCurve(Motions motion, float t)
    {
        motion.positionOffset = new Vector3(
            motion.curveData.positionX.Evaluate(t),
            motion.curveData.positionY.Evaluate(t),
            motion.curveData.positionZ.Evaluate(t));

        motion.rotationOffset = new Vector3(
            motion.curveData.rotationX.Evaluate(t),
            motion.curveData.rotationY.Evaluate(t),
            motion.curveData.rotationZ.Evaluate(t));
    }
}
