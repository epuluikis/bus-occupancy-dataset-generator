using UnityEngine;

public class CameraMarker : MonoBehaviour
{
    public Vector3 size = new(1f, 1f, 1f);
    [Range(0f, 1f)]
    public float weight = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
