using UnityEngine;

public class ProceduralADS : MonoBehaviour
{
    [Header("Camera Settings")]
    public float adsFOV;
    public float defaultFOV;

    [Header("ADS Settings")]
    public Vector3 adsOffset;
    public float transitionSpeed;

    public Camera _camera;
    private Vector3 _originalPosition;
    private float currentFOV;

    private void Start()
    {
        _originalPosition = transform.localPosition;
        currentFOV = _camera.fieldOfView;
    }

    private void Update()
    {
        AddADS();
    }

    private void AddADS()
    {
        Vector3 targetPosition = Input.GetMouseButton(1) ? adsOffset : _originalPosition;

        float targetFOV = Input.GetMouseButton(1) ? adsFOV : defaultFOV;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition, targetPosition, Time.deltaTime * transitionSpeed);

        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * transitionSpeed);

        _camera.fieldOfView = currentFOV;
    }
}
