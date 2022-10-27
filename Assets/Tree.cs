using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // static akan membuat variable shared pada semua Tree
    public static List<Vector3> AllPositions = new List<Vector3>();

    private void OnEnable() {
        AllPositions.Add(this.transform.position);
    }

    private void OnDisable() {
        AllPositions.Remove(this.transform.position);
    }
}
