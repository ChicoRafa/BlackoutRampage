using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    private Vector3 offset = new Vector3(0, 10, -10);
    private float followSpeed = 5f;

    void LateUpdate()
    {
        if (playerTarget is null) return;

        Vector3 desiredPosition = playerTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(playerTarget);
    }
}
