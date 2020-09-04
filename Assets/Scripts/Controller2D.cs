using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {
    [SerializeField] int horizontalRayCount = 4;
    [SerializeField] int verticalRayCount = 4;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private float skinWidth = 0.015f; //How far inside the gameobject the rays originate
    private float maxClimbAngle = 80;
    private float maxDescendAngle = 75;
    [SerializeField]LayerMask layerMask;

    BoxCollider2D collider;
    RayCastOrigins rayCastOrigins;
    public CollisionInfo collisionInfo;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        collisionInfo.Reset();
        UpdateRayCastOrigins();

        if(velocity.y < 0)
        {
            DescentSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }   
        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity) //Refering to the Move(velocity)'s argument Vector3 velocity.
    {
        float directionX = Mathf.Sign(velocity.x); //-1 or 1, moving left is -1, right is 1
        float rayLength = Math.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;
            rayOrigin += Vector2.up *( i * horizontalRaySpacing);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, layerMask);
            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if(!collisionInfo.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;
                    if(collisionInfo.climbingSlope)
                    {
                        print(velocity.y);
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    collisionInfo.collisionLeft = directionX == -1;
                    collisionInfo.collisionRight = directionX == 1;
                }

            }
            Debug.DrawRay(rayOrigin, new Vector2(rayLength * directionX, 0), Color.red);
        }
    }

    void VerticalCollisions(ref Vector3 velocity) //Refering to the Move(velocity)'s argument Vector3 velocity.
    {
        float directionY = Mathf.Sign(velocity.y); //If velocity.y is negative = -1, positive = 1 ;
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft; //Moving up or down?
            rayOrigin += Vector2.right * (i * verticalRaySpacing + velocity.x); //Set the ray origin position where the rays 
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, layerMask); //originate NEXT frame
            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;//if Raycast hits something, set velocity and ray length to the 
                rayLength = hit.distance;                            // ray distance
                if (collisionInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }
                collisionInfo.collisionBelow = directionY == -1;
                collisionInfo.collisionAbove = directionY == 1;
            }
            //Debug.Log(collisionInfo.collisionBelow);
            Debug.DrawRay(rayOrigin, new Vector2(0, rayLength * directionY), Color.blue);
        }

        if(collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1 ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight) + Vector2.up * velocity.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, layerMask);
            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisionInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }

    private void DescentSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomRight : rayCastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, layerMask);
        if(hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <=maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX && !collisionInfo.climbingSlope)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = moveDistance * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
                        velocity.x = moveDistance * Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.collisionBelow = true;
                    }
                }
            }
        }
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Math.Abs(velocity.x);
        float climbVelocityY = moveDistance * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);
        
        if (velocity.y <= climbVelocityY)
        {
            collisionInfo.climbingSlope = true;
            velocity.y = climbVelocityY;
            velocity.x = moveDistance * Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
            collisionInfo.collisionBelow = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
        
    }



    private void DrawRays()
    {
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Debug.DrawRay(new Vector2(rayCastOrigins.bottomRight.x, rayCastOrigins.bottomRight.y + (i * horizontalRaySpacing)), Vector2.right, Color.red);
        }
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRayCastOrigins()
    {
        Bounds bounds = collider.bounds; //Get the bounds of a boxcollider of the gameobject
        bounds.Expand(skinWidth * -2);

        rayCastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y); //Set the box collider corners to RayCastOrigins struct reference
        rayCastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        rayCastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayCastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    

}
public struct RayCastOrigins
{
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
}

public struct CollisionInfo
{
    public bool collisionBelow, collisionAbove;
    public bool collisionRight, collisionLeft;

    public bool descendingSlope;
    public bool climbingSlope;
    public float slopeAngle, slopeAngleOld;


    public void Reset()
    {
        collisionBelow = false;
        collisionAbove = false;
        collisionLeft = false;
        collisionRight = false;

        climbingSlope = false;

        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
    }
}
