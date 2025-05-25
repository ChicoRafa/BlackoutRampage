using System.Collections.Generic;
using UnityEngine;

public class Shelving : MonoBehaviour
{
    [Header("Lists")]
    public List<Transform> objectsPositionsList = new List<Transform>();
    public List<GameObject> objectsList = new List<GameObject>();

    [Header("Object Type")]
    public string objectType;
}
