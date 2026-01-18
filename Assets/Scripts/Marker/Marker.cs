using UnityEngine;

public abstract class Marker : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Gizmos.color = Color();
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    protected abstract Color Color();
}
