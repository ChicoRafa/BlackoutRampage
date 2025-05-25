using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTarget;

    [Header("Smooth Speed")]
    [SerializeField] private float smoothSpeed = 10f;

    private void LateUpdate()
    {
        if (!playerTarget) return; 
        transform.position = Vector3.Lerp(transform.position, playerTarget.position, smoothSpeed * Time.deltaTime);
    }
}
