using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The object the camera will follow
    public float smoothSpeed = 1f; // Smooth movement speed
    public Vector3 offset;          // Offset from the target

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
