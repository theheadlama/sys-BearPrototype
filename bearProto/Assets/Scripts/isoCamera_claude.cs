// using UnityEngine;

// public class IsometricCameraController : MonoBehaviour
// {
//     [Header("Target Settings")]
//     [SerializeField] private Transform target;
//     [SerializeField] private Vector3 offset = new Vector3(7f, 8f, -7f);
    
//     [Header("Follow Settings")]
//     [SerializeField] private float smoothSpeed = 5f;
//     [SerializeField] private bool instantFollow = false;
    
//     [Header("Rotation Settings")]
//     [SerializeField] private Vector3 rotation = new Vector3(35f, -45f, 0f);
    
//     [Header("Boundaries")]
//     [SerializeField] private bool useBoundaries = false;
//     [SerializeField] private float minX = -50f;
//     [SerializeField] private float maxX = 50f;
//     [SerializeField] private float minZ = -50f;
//     [SerializeField] private float maxZ = 50f;

//     private void Start()
//     {
//         if (target == null)
//         {
//             Debug.LogWarning("No target assigned to IsometricCameraController!");
//             return;
//         }

//         // Set initial rotation for isometric view
//         transform.rotation = Quaternion.Euler(rotation);
        
//         if (instantFollow)
//         {
//             transform.position = target.position + offset;
//         }
//     }

//     private void LateUpdate()
//     {
//         if (target == null) return;

//         // Calculate desired position
//         Vector3 desiredPosition = target.position + offset;
        
//         // Apply boundaries if enabled
//         if (useBoundaries)
//         {
//             desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
//             desiredPosition.z = Mathf.Clamp(desiredPosition.z, minZ, maxZ);
//         }

//         // Smoothly move camera to desired position
//         if (instantFollow)
//         {
//             transform.position = desiredPosition;
//         }
//         else
//         {
//             Vector3 smoothedPosition = Vector3.Lerp(
//                 transform.position,
//                 desiredPosition,
//                 smoothSpeed * Time.deltaTime
//             );
//             transform.position = smoothedPosition;
//         }
//     }

//     // Public method to set new target at runtime
//     public void SetTarget(Transform newTarget)
//     {
//         target = newTarget;
//     }

//     // Public method to adjust offset at runtime
//     public void SetOffset(Vector3 newOffset)
//     {
//         offset = newOffset;
//     }

//     // Optional: Visualize the boundaries in the editor
//     private void OnDrawGizmosSelected()
//     {
//         if (!useBoundaries) return;
        
//         Gizmos.color = Color.yellow;
//         Vector3 center = new Vector3((minX + maxX) / 2f, transform.position.y, (minZ + maxZ) / 2f);
//         Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
//         Gizmos.DrawWireCube(center, size);
//     }
// }
