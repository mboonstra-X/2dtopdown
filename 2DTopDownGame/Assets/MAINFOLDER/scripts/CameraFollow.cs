using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = -10f; // camera Z
        transform.position = targetPosition;
    }
}
