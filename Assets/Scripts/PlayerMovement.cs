using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {
    [SerializeField] float timeToJumpApex = 0.4f;
    [SerializeField] float jumpHeight;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float accelerationTimeGrounded = 0.1f;
    [SerializeField] float accelerationTimeAirborne = 1f;
    float xMoveSmoothing;
    float gravity;
    float jumpVelocity;
    Vector3 velocity;

    Controller2D controller;
    private void Start()
    {
        controller = GetComponent<Controller2D>();
        gravity = -Mathf.Abs((2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2));
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
    }

    private void Update()
    {
        
        if(controller.collisionInfo.collisionAbove || controller.collisionInfo.collisionBelow)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisionInfo.collisionBelow)
        {
            velocity.y = jumpVelocity;
        }
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xMoveSmoothing, GetSmoothTime());
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private float GetSmoothTime()
    {
        return controller.collisionInfo.collisionBelow == true ? accelerationTimeGrounded : accelerationTimeAirborne;
    }
}

