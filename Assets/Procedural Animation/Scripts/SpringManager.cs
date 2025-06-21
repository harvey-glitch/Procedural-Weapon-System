using UnityEngine;

// just add more motion here if needed
    #region List of Motion Classes
[System.Serializable]
public class RecoilSettings
{
    [Tooltip("Backward vector applied to the weapon when firing")]
    public float Kickback = 2f;

    [Tooltip("Upward rotation applied to the weapon when firing")]
    public float XRotation = 30f;
}

[System.Serializable]
public class BobSettings
{
    [Tooltip("How strong the horizontal oscillation is")]
    public float XAmplitude = 0.01f;

    [Tooltip("How strong the vertical oscillation is")]
    public float YAmplitude = 0.02f;

    [Tooltip("The amount the tilting when oscillating")]
    public float TiltAmount = 3f;

    [Tooltip("How fast the oscillation moves")]
    public float Frequency = 8f;

    [Tooltip("Multiplier to scale the overall oscillation")]
    public float Multiplier = 2f;
}

[System.Serializable]
public class BreathSettings
{
    [Tooltip("How strong the horizontal oscillation is")]
    public float XAmplitude = 0.01f;

    [Tooltip("How strong the vertical oscillation is")]
    public float YAmplitude = 0.005f;

    [Tooltip("How fast the horizontal oscillation moves")]
    public float XFrequency = 0.1f;

    [Tooltip("How fast the vertical oscillation moves")]
    public float YFrequency = 0.75f;
}

[System.Serializable]
public class SwaySettings
{
    [Tooltip("How much sway is applied based on mouse input")]
    public float amount = 0.01f;

    [Tooltip("The maximum limit the sway can reach.")]
    public float maximum = 0.1f;
}

[System.Serializable]
public class SprintSettings
{
    [Tooltip("The rotation of the weapon when sprinting")]
    public Vector3 SprintRotation;

    [Tooltip("The position of the weapon when sprinting")]
    public Vector3 SprintPosition;
}
#endregion

public class SpringManager : MonoBehaviour
{
    #region Motion Offsets Container
    public struct MotionOffset
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public MotionOffset(Vector3 targetPosition, Vector3 targetRotation)
        {
            Position = targetPosition;
            Rotation = targetRotation;
        }

        public static MotionOffset operator +(MotionOffset a, MotionOffset b)
        {
            return new MotionOffset(a.Position + b.Position, a.Rotation + b.Rotation);
        }
    }
    #endregion
  
    public static SpringManager instance;

    [Header("Motion Configurations")]
    public RecoilSettings RecoilMotionSettings;
    public BobSettings BobMotionSettings;
    public BreathSettings BreathMotionSettings;
    public SwaySettings SwayMotionSettings;
    public SprintSettings SprintMotionSettings;

    // reference to the fps controller script
    private FPSController _fpsController;

    // timer used to calculate the bobbing effect while moving
    private float bobTimer;

    // timer used to calculate the breathing effect when not moving
    private float breathTimer;

    private void Awake()
    {
        _fpsController ??= FindFirstObjectByType<FPSController>();

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

    private void Update()
    {
        bool isMoving = GetMoveInput().sqrMagnitude >= 0.0001f;

        AddRecoil();
        AddBob(isMoving);
        AddBreath(isMoving);
        AddSway();
        AddSprint();
    }

    public void AddRecoil()
    {
        if (Input.GetMouseButton(0))
        {
            // check if the recoil is completed before allowing another again
            // useful for non accumulative weapon like pistol or shotgun
            // remove this condition for accumulative recoil
            if (SpringSystem.instance.GetPositionSpring("Recoiling").IsAtRest())
            {
                MotionOffset recoilResult = GetRecoilOffset(RecoilMotionSettings);

                SpringSystem.instance.AddImpulseForce(
                    "Recoiling", recoilResult.Position, recoilResult.Rotation * 20f);
            }
        }
    }

    public void AddBob(bool isMoving)
    {
        // advance or reset the bob timer based on is moving variable
        bobTimer = isMoving ? bobTimer += Time.deltaTime : 0f;

        // adjust bob strength based on movement direction
        float multiplier = _fpsController.IsMovingForward() ? BobMotionSettings.Multiplier : 1f;

        MotionOffset bobResult = GetBobOffset(bobTimer, multiplier, BobMotionSettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Bobbing", bobResult.Position, bobResult.Rotation);
    }

    public void AddBreath(bool isMoving)
    {
        // advance or reset the breath timer based on is not moving variable
        breathTimer = isMoving ? 0f : breathTimer += Time.deltaTime;

        MotionOffset breathResult = GetBreathOffset(breathTimer, BreathMotionSettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Breathing", breathResult.Position, breathResult.Rotation);
    }

    public void AddSway()
    {
        MotionOffset swayResult = GetSwayOffset(SwayMotionSettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Swaying", swayResult.Position, swayResult.Rotation);
    }

    public void AddSprint()
    {
        MotionOffset sprintResult = GetSprintOffset(SprintMotionSettings);

        SpringSystem.instance.AddConstantForce(
            "Sprinting", sprintResult.Position, sprintResult.Rotation);
    }

    #region Methods for Generating Motions
    private MotionOffset GetRecoilOffset(RecoilSettings settings)
    {
        // creates a backward movement along the z axis
        Vector3 recoilPosition = new Vector3(
            0f, 0f, -settings.Kickback);

        // create a downward vertical rotation on the x axis
        Vector3 recoilRotation = new Vector3(
            -settings.XRotation, 0f, 0f);

        return new MotionOffset(recoilPosition, recoilRotation);
    }

    private MotionOffset GetBobOffset(float time, float multiplier, BobSettings settings)
    {
        // create a side to side and a downward movement for x and y position
        Vector3 bobPosition = new Vector3(
            Mathf.Sin(time * settings.Frequency) * settings.XAmplitude * multiplier,
            -Mathf.Abs(Mathf.Sin(time * settings.Frequency)) * settings.YAmplitude * multiplier,
            0f);

        // create a side to side movement for z axis rotation
        Vector3 bobRotation = new Vector3(
            0f, 0f, Mathf.Sin(time * settings.Frequency * 2f) * -settings.TiltAmount * multiplier);

        return new MotionOffset(bobPosition, bobRotation);
    }

    private MotionOffset GetBreathOffset(float time, BreathSettings settings)
    {
        // Create a randomized offset for the x position and an oscillation for the y position.
        Vector3 breathPosition = new Vector3(
            (Mathf.PerlinNoise(time * settings.XFrequency, 0.5f) * 2f - 1f) * settings.XAmplitude,
            Mathf.Sin(time * settings.YFrequency + Mathf.PI / 2f) * settings.YAmplitude,
            0f);

        return new MotionOffset(breathPosition, Vector3.zero);
    }

    private MotionOffset GetSwayOffset(SwaySettings settings)
    {
        // create a position based on current mouse position
        Vector3 swayPosition = new Vector3(
            -GetMouseInput().x, -GetMouseInput().y, 0f) * settings.amount;

        // clapmed the position to max sway to avoid overshooting
        Vector3 clapmedSwayPosition = Vector3.ClampMagnitude(swayPosition, settings.maximum);

        return new MotionOffset(clapmedSwayPosition, Vector3.zero);
    }

    private MotionOffset GetSprintOffset(SprintSettings settings)
    {
        if (_fpsController.IsMovingForward())
        {
            return new MotionOffset(settings.SprintPosition, settings.SprintRotation);
        }
        else
        {
            return new MotionOffset(Vector3.zero, Vector3.zero);
        }
    }
    #endregion

    #region Inputs Methods
    private Vector3 GetMouseInput()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private Vector3 GetMoveInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }
    #endregion
}
