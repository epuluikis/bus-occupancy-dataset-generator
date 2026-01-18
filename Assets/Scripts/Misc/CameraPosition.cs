using System;
using UnityEngine;

[Serializable]
public class CameraPosition
{
    public Vector3 position;
    public Quaternion rotation;

    public CameraPosition(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public CameraPosition(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}
