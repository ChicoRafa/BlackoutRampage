using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Player/Parameters/MovementConfig")]
public class PlayerMovementConfig : ScriptableObject
{
    public float walkSpeed = 500f;
    public float sprintSpeed = 650f;
    public float rotationSpeed = 10f;
}