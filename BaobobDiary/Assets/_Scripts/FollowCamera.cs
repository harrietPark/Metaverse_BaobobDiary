using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // The target (camera) to follow
    public float distance = 1f; // Distance in front of the target
    public Vector3 offset; // Offset from the target
    public float followSmoothTime = 0.2f; // Speed of smoothing for following
    public float rotationSmoothTime = 0.2f; // Speed of smoothing for rotation

    private Vector3 velocity = Vector3.zero; // Velocity used by SmoothDamp

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position
        Vector3 targetPosition = target.position + target.forward * distance + offset;

        // Smoothly move to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSmoothTime);

        // Smoothly rotate to match the target's rotation
        Quaternion targetRotation = Quaternion.LookRotation(target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
    }
}
