using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // a simple controller to show why to use the system

    [Header("REFERENCES")]
    public FPSController fpsController;

    private void Start()
    {
        WeaponAnimator.instance.PlayMotionOnce("Draw");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // play the draw animation once when not draw is not playing
            WeaponAnimator.instance.PlayMotionOnce("Recoil", "Draw");
        }

        // basically your saying here, play bob animation in looping pattern
        // if the player is moving, and either of these three are playing (idle, recoil, draw etc)
        WeaponAnimator.instance.PlayMotionLoop("Bob", fpsController.IsMoving(), "Idle", "Recoil", "Draw");
        WeaponAnimator.instance.PlayMotionLoop("Idle", !fpsController.IsMoving(), "Bob", "Recoil", "Draw");
    }
}
