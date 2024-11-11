using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    
    // vars to hold input actions using new input system (wait on JumpAction for now)
    InputAction moveAction;
//    InputAction jumpAction;
    
    // Movement speed in units per second
    [SerializeField]
    private float moveSpeed = 5f;

    // Optional: For smoother movement
    [SerializeField]
    private float smoothTime = 0.1f;

    [SerializeField]
    private float rotationSpeed = 10.0f;

    // Reference to the Rigidbody component
    private Rigidbody rb;

    // Current velocity for smoothing
    private Vector3 currentVelocity = Vector3.zero;

    // Cached transform for better performance
    private Transform cachedTransform;
    private Vector3 movement;

    private void Start()
    {
        // initialize input vars
        moveAction = InputSystem.actions.FindAction("Move");
     //   jumpAction = InputSystem.actions.FindAction("Jump");
        
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        cachedTransform = transform;

        // Optional: Freeze rotation to prevent tipping
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        bool controlInput = false;
        
        // maintain forward vector if no input
        if( moveValue.x != 0 || moveValue.y != 0) {
            controlInput = true;
        }
   
        //   Calculate new movement direction
        movement = new Vector3(moveValue.x, 0f, moveValue.y).normalized;

        if (rb != null)
        {
            // Physics-based movement
            MoveWithPhysics(movement);
        }
        RotateToMatch(movement, controlInput);
    }

    private void MoveWithPhysics(Vector3 movement)
    {
        // Calculate target velocity
        Vector3 targetVelocity = movement * moveSpeed;

        // Apply smooth movement using physics
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);
    }


    private void RotateToMatch(Vector3 movement, bool controlInput) {
        
        if( controlInput == false) {
            return;
        }

        //calculate rotation
        Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
        
        //rotate the object to face the movement direction
        cachedTransform.rotation = Quaternion.Lerp(cachedTransform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);

    }
}