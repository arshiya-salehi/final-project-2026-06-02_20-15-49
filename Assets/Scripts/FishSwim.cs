using UnityEngine;

public class FishSwim : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Movement speed of the fish.")]
    [SerializeField] private float speed = 1.0f;
    [Tooltip("Rotation speed towards the target.")]
    [SerializeField] private float turnSpeed = 2.0f;
    [Tooltip("The radius within which the fish will swim.")]
    [SerializeField] private float swimRadius = 5.0f;
    [Tooltip("The vertical range for swimming.")]
    [SerializeField] private float depthRange = 1.0f;
    [Tooltip("The center point of the swimming area.")]
    [SerializeField] private Vector3 centerPosition;

    private Vector3 _targetPoint;
    private Transform _forcedTarget;

    private void Start()
    {
        // If centerPosition is not set, use current position
        if (centerPosition == Vector3.zero) 
            centerPosition = transform.position;
        
        UpdateTargetPoint();
    }

    private void Update()
    {
        // If we have a forced target (the hook), update target point to its position
        if (_forcedTarget != null)
        {
            _targetPoint = _forcedTarget.position;
        }

        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Calculate direction to target
        Vector3 direction = _targetPoint - transform.position;
        
        if (direction.magnitude > 0.1f)
        {
            // Rotate smoothly towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Update target if reached (only if not forced)
        if (_forcedTarget == null && Vector3.Distance(transform.position, _targetPoint) < 0.5f)
        {
            UpdateTargetPoint();
        }
    }

    public void SetForcedTarget(Transform target)
    {
        _forcedTarget = target;
    }

    private void UpdateTargetPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * swimRadius;
        float randomDepth = Random.Range(-depthRange, depthRange);
        _targetPoint = centerPosition + new Vector3(randomCircle.x, randomDepth, randomCircle.y);
    }

    /// <summary>
    /// Programmatically set the swimming parameters.
    /// </summary>
    public void Setup(float swimSpeed, float rotationSpeed, float radius, float depth, Vector3 center)
    {
        speed = swimSpeed;
        turnSpeed = rotationSpeed;
        swimRadius = radius;
        depthRange = depth;
        centerPosition = center;
    }
}
