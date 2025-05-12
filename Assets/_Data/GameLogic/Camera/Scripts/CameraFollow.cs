using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private CameraSettings cameraSettings;

    private void LateUpdate()
    {
        if (!playerTarget || !cameraSettings) return;

        Vector3 targetPosition = playerTarget.position + cameraSettings.offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSettings.followSpeed * Time.deltaTime);
    }
}