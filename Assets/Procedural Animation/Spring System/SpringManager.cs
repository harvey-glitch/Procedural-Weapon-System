using UnityEngine;

public class SpringManager : MonoBehaviour
{
    #region Motion Offset Container
    public struct MotionOffset
    {
        public Vector3 position;
        public Vector3 rotation;

        public MotionOffset(Vector3 pos, Vector3 rot)
        {
            position = pos;
            rotation = rot;
        }

        public static MotionOffset operator +(MotionOffset a, MotionOffset b)
        {
            return new MotionOffset(a.position + b.position, a.rotation + b.rotation);
        }
    }

    #endregion

    #region Motion Serializables
    [System.Serializable]
    public class BobSettings { public float xAmplitude, yAmplitude, frequency; }

    [System.Serializable]
    public class RecoilSettings { public float kickback, xRotation, yRotation; }

    [System.Serializable]
    public class BreathSettings { public float xAmplitude, yAmplitude, xFrequency, yFrequency; }

    [System.Serializable]
    public class SwaySettings { public float amount, maximum; }
    #endregion

    public static SpringManager instance;

    [Header("Animation configuration")]
    public BobSettings bobSettings;

    [Space(5)]
    public RecoilSettings recoilSettings;

    [Space(5)]
    public BreathSettings breathSettings;

    [Space(5)]
    public SwaySettings swaySettings;

    private float bobTimer;
    private float breathTimer;

    private void Awake()
    {
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
        AddRecoil();
        AddBob();
        AddBreath();
        AddSway();
    }

    public void AddRecoil()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MotionOffset recoilResult = GetRecoilOffset(recoilSettings);

            // call the method for adding instant / impulse force in the spring manager
            SpringSystem.instance.SetInstantForce(
                "Recoiling", recoilResult.position, recoilResult.rotation);
        }
    }

    public void AddBob()
    {
        // advance the bob timer if theres an key input, reset if none
        bobTimer = GetMoveInput().sqrMagnitude >= 0.0001f ? bobTimer += Time.deltaTime : 0f;

        MotionOffset bobResult = GetBobOffset(bobTimer, bobSettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Bobbing", bobResult.position, bobResult.rotation);
    }

    public void AddBreath()
    {
        // advance the breath timer if theres no key input detected, reset if theres one
        breathTimer = GetMoveInput().sqrMagnitude < 0.0001f ? breathTimer += Time.deltaTime : 0f;

        MotionOffset breathResult = GetBreathOffset(breathTimer, breathSettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Breathing", breathResult.position, breathResult.rotation);
    }

    public void AddSway()
    {
        MotionOffset swayResult = GetSwayOffset(swaySettings);

        // call the method for adding constant / continues force in the spring manager
        SpringSystem.instance.AddConstantForce(
            "Swaying", swayResult.position, swayResult.rotation);
    }

    #region Methods for Generating Motions
    private MotionOffset GetRecoilOffset(RecoilSettings settings)
    {
        // creates a backward movement along the Z-axis
        Vector3 recoilPosition = new Vector3(
            0f, 0f, -settings.kickback);

        // create a downward vertical rotation on the x axis
        // and randomized left and right horizontal rotation for y axis
        Vector3 recoilRotation = new Vector3(
            -settings.xRotation, Random.Range(-settings.yRotation, settings.yRotation), 0f);

        return new MotionOffset(recoilPosition, recoilRotation);
    }

    private MotionOffset GetBobOffset(float time, BobSettings settings)
    {
        // create a left and right horizontal movement for x-axis
        // and a downward vertical movement on the Y-axis
        Vector3 bobPosition = new Vector3(
            Mathf.Sin(time * settings.frequency) * settings.xAmplitude,
            -Mathf.Abs(Mathf.Sin(time * settings.frequency)) * settings.yAmplitude * 0.5f,
            0f);

        // create a left and right horizontal rotation (tilting) for the z axis
        Vector3 bobRotation = new Vector3(
            0f, 0f, Mathf.Sin(time * settings.frequency * 2f) * -2f);

        return new MotionOffset(bobPosition, bobRotation);
    }

    private MotionOffset GetBreathOffset(float time, BreathSettings settings)
    {
        // create a randomized offset for x axis
        // and a sine wave oscillation starting from bottom for y axis
        Vector3 breathPosition = new Vector3(
            (Mathf.PerlinNoise(time * settings.xFrequency, 0.5f) * 2f - 1f) * settings.xAmplitude,
            Mathf.Sin(time * settings.yFrequency + Mathf.PI / 2f) * settings.yAmplitude,
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

    #endregion

    #region Inputs
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
