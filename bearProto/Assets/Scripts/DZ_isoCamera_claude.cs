using UnityEngine;

public class DZ_isoCamera_claude : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(10f, 14f, -10f);
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool instantFollow = false;
    
    [Header("Dead Zone Settings")]
    [SerializeField] private bool useDeadZone = true;
    [SerializeField] private float horizontalDeadZone = 2f;
    [SerializeField] private float verticalDeadZone = 3f;
    [SerializeField] private float deadZoneSmoothTime = 0.1f;
    
    [Header("Recentering Settings")]
   [SerializeField] private bool enableRecentering = true;  // Toggle for recentering feature
   [SerializeField] private float recenterDelay = 0.5f;
    [SerializeField] private float recenterThreshold = 0.1f;
    [SerializeField] [Range(0.1f, 10f)] private float recenterMaxSpeed = 5f;
    [SerializeField] [Range(0.1f, 10f)] private float recenterAcceleration = 2f;
    [SerializeField] [Range(0.1f, 10f)] private float recenterDeceleration = 3f;
    [SerializeField] private bool inheritVelocityOnRecenter = true;
    [SerializeField] [Range(0f, 1f)] private float inheritedVelocityMultiplier = 0.5f;
    [SerializeField] private bool visualizeDeadZone = true;
    [SerializeField] private Color deadZoneColor = new Color(0f, 1f, 0f, 0.3f);
    
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotation = new Vector3(35f, -45f, 0f);
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;

    private Vector2 lastTargetPosition2D;
    private Vector2 targetVelocity;
    private Vector2 currentDeadZoneCenter;
    private Vector2 recenterVelocity;
    private Vector2 lastDeadZonePosition;
    private Vector2 deadZoneVelocity;
    private float currentRecenterSpeed;
    private float timeSinceMovement;
    private bool isRecentering;
    private bool hasInheritedVelocity;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to DZ_isoCamera_claude!");
            return;
        }

        transform.rotation = Quaternion.Euler(rotation);
        Vector2 targetPos2D = new Vector2(target.position.x, target.position.z);
        lastTargetPosition2D = targetPos2D;
        currentDeadZoneCenter = targetPos2D;
        lastDeadZonePosition = currentDeadZoneCenter;
        
        if (instantFollow)
        {
            transform.position = target.position + offset;
        }
    }

    private float GetEllipticalDistance(Vector2 point, Vector2 center)
    {
        float dx = (point.x - center.x) / horizontalDeadZone;
        float dy = (point.y - center.y) / verticalDeadZone;
        return new Vector2(dx, dy).magnitude;
    }

    private Vector2 UpdateRecenteringPosition(Vector2 current, Vector2 target, float deltaTime)
    {
        Vector2 toTarget = target - current;
        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget < 0.01f)
        {
            currentRecenterSpeed = 0f;
            return target;
        }

        // Initialize recentering speed with inherited velocity if enabled
        if (inheritVelocityOnRecenter && !hasInheritedVelocity)
        {
            deadZoneVelocity = (currentDeadZoneCenter - lastDeadZonePosition) / deltaTime;
            float inheritedSpeed = deadZoneVelocity.magnitude * inheritedVelocityMultiplier;
            currentRecenterSpeed = Mathf.Min(inheritedSpeed, recenterMaxSpeed);
            hasInheritedVelocity = true;
        }

        // Update speed based on distance to target
        float normalizedDistance = Mathf.Clamp01(distanceToTarget / horizontalDeadZone);
        
        if (normalizedDistance > 0.5f)
        {
            // Accelerate
            currentRecenterSpeed += recenterAcceleration * deltaTime;
        }
        else
        {
            // Decelerate as we approach target
            float decelerationFactor = normalizedDistance * 2f;
            currentRecenterSpeed -= recenterDeceleration * decelerationFactor * deltaTime;
        }

        // Clamp speed
        currentRecenterSpeed = Mathf.Clamp(currentRecenterSpeed, 0f, recenterMaxSpeed);

        // Calculate movement
        Vector2 moveDirection = toTarget.normalized;
        Vector2 movement = moveDirection * currentRecenterSpeed * deltaTime;

        // Don't overshoot
        if (movement.magnitude > distanceToTarget)
        {
            return target;
        }

        return current + movement;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 targetPos2D = new Vector2(target.position.x, target.position.z);
        lastDeadZonePosition = currentDeadZoneCenter; // Store last position for velocity calculation
        Vector2 newDeadZoneCenter = currentDeadZoneCenter;

        // Calculate target velocity
        targetVelocity = (targetPos2D - lastTargetPosition2D) / Time.deltaTime;
        float speed = targetVelocity.magnitude;

        // Update movement timer
        if (speed > recenterThreshold)
        {
            timeSinceMovement = 0f;
            isRecentering = false;
            hasInheritedVelocity = false;
            currentRecenterSpeed = 0f;
        }
        else
        {
            timeSinceMovement += Time.deltaTime;
        }

         if (useDeadZone)
        {
            if (enableRecentering && timeSinceMovement >= recenterDelay)
            {
                // Handle recentering
                isRecentering = true;
                newDeadZoneCenter = UpdateRecenteringPosition(
                    currentDeadZoneCenter,
                    targetPos2D,
                    Time.deltaTime
                );
            }
            else
            {
                // Handle dead zone following
                isRecentering = false;
                float distanceRatio = GetEllipticalDistance(targetPos2D, currentDeadZoneCenter);

                if (distanceRatio > 1f)
                {
                    Vector2 toTarget = targetPos2D - currentDeadZoneCenter;
                    toTarget.Normalize();

                    float excessDistance = GetEllipticalDistance(targetPos2D, currentDeadZoneCenter) - 1f;
                    Vector2 movement = toTarget * excessDistance;

                    newDeadZoneCenter = Vector2.SmoothDamp(
                        currentDeadZoneCenter,
                        currentDeadZoneCenter + movement,
                        ref recenterVelocity,
                        deadZoneSmoothTime
                    );
                }
            }
        }
        else
        {
            newDeadZoneCenter = targetPos2D;
        }

        // Update positions
        currentDeadZoneCenter = newDeadZoneCenter;
        lastTargetPosition2D = targetPos2D;

        // Calculate camera position based on dead zone center
        Vector3 newCameraTargetPosition = new Vector3(
            currentDeadZoneCenter.x,
            target.position.y,
            currentDeadZoneCenter.y
        );

        // Calculate final camera position
        Vector3 desiredPosition = newCameraTargetPosition + offset;
        
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
        }

        // Move camera
        if (instantFollow)
        {
            transform.position = desiredPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
        }
    }

   private void OnDrawGizmos()
    {
        if (!visualizeDeadZone || !useDeadZone || target == null) return;

        // Get camera rotation
        Quaternion cameraRotation = transform.rotation;
        Vector3 eulerAngles = cameraRotation.eulerAngles;
        Quaternion groundAlignedRotation = Quaternion.Euler(0, eulerAngles.y, 0);

        // Draw elliptical dead zone
        Gizmos.color = deadZoneColor;
        Vector3 center = new Vector3(currentDeadZoneCenter.x, target.position.y, currentDeadZoneCenter.y);
        
        // Draw ellipse approximation
        int segments = 64;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            // Create points in local space
            Vector3 point1Local = new Vector3(
                horizontalDeadZone * Mathf.Cos(angle1),
                0f,
                verticalDeadZone * Mathf.Sin(angle1)
            );
            
            Vector3 point2Local = new Vector3(
                horizontalDeadZone * Mathf.Cos(angle2),
                0f,
                verticalDeadZone * Mathf.Sin(angle2)
            );
            
            // Rotate points to match camera orientation
            Vector3 point1 = center + groundAlignedRotation * point1Local;
            Vector3 point2 = center + groundAlignedRotation * point2Local;
            
            Gizmos.DrawLine(point1, point2);
        }

        // Draw axes of the ellipse
        Gizmos.color = Color.red;
        // Horizontal axis
        Vector3 rightAxis = groundAlignedRotation * Vector3.right * horizontalDeadZone;
        Gizmos.DrawLine(center - rightAxis, center + rightAxis);
        
        // Vertical axis
        Vector3 forwardAxis = groundAlignedRotation * Vector3.forward * verticalDeadZone;
        Gizmos.DrawLine(center - forwardAxis, center + forwardAxis);

        // Draw velocities
        if (Application.isPlaying)
        {
            // Draw dead zone velocity
            if (deadZoneVelocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Vector3 velocityLine = new Vector3(deadZoneVelocity.x, 0, deadZoneVelocity.y).normalized * 2f;
                Gizmos.DrawLine(center, center + velocityLine);
            }

            // Draw recentering speed
            if (isRecentering && currentRecenterSpeed > 0.1f)
            {
                Gizmos.color = Color.yellow;
                Vector3 toTarget = target.position - center;
                Vector3 recenterLine = toTarget.normalized * currentRecenterSpeed;
                Gizmos.DrawLine(center, center + recenterLine);
            }

            // Draw target line
            Gizmos.color = Color.white;
            Gizmos.DrawLine(center, target.position);
        }
    }
}