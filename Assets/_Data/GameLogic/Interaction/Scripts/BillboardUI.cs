using System;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    Camera mainCamera = Camera.main;
    private void LateUpdate()
    {
        if (!mainCamera) return;
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180f, 0);
    }
}
