using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Camera/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public Vector3 offset = new Vector3(0, 3, -6);
    public float followSpeed = 5f;
}
