using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    // Temp settings
    public float maxSteerForce = 5.0f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;
    public float startSpeed = 1.0f;
    public float radius = 3.0f;
    public float cohesionForce = 1.0f;
    public float alignmentForce = 1.0f;
    public float separationForce = 1.0f;
    public LayerMask collisionLayer;
    public float boundsRadius = .27f;
    public float collisionAvoidDst = 2;
    public float avoidCollisionForce = 5;

    // Flock that the boid belongs to
    public BoidGroup group;
    

    // Current frame variables
    public BoidGroup flock;

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 forward;
    public Vector3 acceleration;
    public Transform cachedTransform;


    public void Awake()
    {
        cachedTransform = transform;
    }

    public void Initialize(BoidGroup groupToJoin)
    {
        group = groupToJoin;
        position = cachedTransform.position;
        forward = cachedTransform.forward;
        velocity = forward * startSpeed;
    }

    public void Update()
    {
        flock = group.FindBoidsInRange(this, radius);
        int flockSize = flock.Size;

        acceleration = Vector3.zero;

        if (flockSize > 0)
        {
            ApplyCohesion();
            ApplyAlignment();
            ApplySeparation();
        }

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            //Debug.Log(collisionAvoidDir * avoidCollisionForce);
            acceleration += SteerTowards(collisionAvoidDir) * avoidCollisionForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;

        Vector3 dir = velocity / speed;

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;

        position = cachedTransform.position;
        forward = cachedTransform.forward;
    }

    public void ApplyCohesion() {
        Vector3 centerOfFlock = flock.CalculateAveragePosition();
        Vector3 offsetFromCenterOfFlock = centerOfFlock - transform.position;

        acceleration += SteerTowards(offsetFromCenterOfFlock) * cohesionForce;
    }

    public void ApplyAlignment()
    {
        Vector3 averageHeading = flock.CalculateAverageHeading();
        acceleration += SteerTowards(averageHeading) * alignmentForce;
    }

    public void ApplySeparation()
    {
        Vector3 separation = flock.CalculateAvoidance(this);
        acceleration += SteerTowards(separation) * separationForce;
    }

    public Vector3 SteerTowards(Vector3 direction)
    {
        Vector3 turnVector = direction.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(turnVector, maxSteerForce);
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, boundsRadius, forward, out hit, collisionAvoidDst, collisionLayer))
        {
            return true;
        }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, boundsRadius, collisionAvoidDst, collisionLayer))
            {
                return dir;
            }
        }
        return forward;
    }

    private void OnDrawGizmos()
    {

        //Gizmos.color = Color.magenta;
        //foreach (Boid boid in flock.members)
        //{
        //    Gizmos.DrawLine(position, boid.position);
        //}

        Gizmos.DrawRay(position, flock.CalculateAvoidance(this) * separationForce);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(position, cachedTransform.forward * collisionAvoidDst);

        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.1f);
        Gizmos.DrawSphere(position, radius);

        if (IsHeadingForCollision())
        {

            Vector3[] rayDirections = BoidHelper.directions;
            for (int i = 0; i < rayDirections.Length; i++)
            {
                Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
                Ray ray = new Ray(position, dir);
                if (!Physics.SphereCast(ray, boundsRadius, collisionAvoidDst, collisionLayer))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(position, dir * collisionAvoidDst);
                    break;
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(position, dir * collisionAvoidDst);
                }
            }
        }
    }
    }
