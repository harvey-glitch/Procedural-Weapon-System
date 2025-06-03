using UnityEngine;

public static class InputManager
{
    public static Vector3 GetMoveInput()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        return new Vector3(xInput, 0, zInput);
    }

    public static Vector2 GetLookInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        return new Vector2(mouseX, mouseY);
    }

    public static bool GetAimInput()
    {
        return Input.GetMouseButtonDown(1);
    }
}
