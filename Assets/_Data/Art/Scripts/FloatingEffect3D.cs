using UnityEngine;

public class FloatingEffect3D : MonoBehaviour
{
    [Header("Effects values")]
    public float floatSpeed = 0.5f;
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;

    private Camera mainCamera;
    private Vector3 localOffset = Vector3.up;
    public float progress = 0f;

    private void Start()
    {
        mainCamera = Camera.main;
        transform.localScale = startScale;
    }

    private void Update()
    {
        if (transform.parent != null)
            transform.position = transform.parent.position + localOffset * (progress * floatSpeed);

        float t = Mathf.Clamp01(progress);
        transform.localScale = Vector3.Lerp(startScale, endScale, t);

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
