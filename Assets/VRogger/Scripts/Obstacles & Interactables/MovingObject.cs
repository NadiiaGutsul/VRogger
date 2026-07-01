using System;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    public float Length { get; private set; }

    private float speed;
    private float direction;
    private Quaternion rotation;
    private bool isStopped = false;

    private float threshhold;

    private float detectionDistance;

    private Vector3 moveDirection => -transform.right;


    private LayerMask vrInteractableLayer;
    private LayerMask movingObjectLayer;
    private String occupiedTag;

    public void Initialise(float speed, float direction) {
        this.speed = speed;
        this.direction = direction;
    }

    private void Awake()
    {
        Length = GetComponent<Collider>().bounds.size.x;
    }


    void Start()
    {
        // transform.localscale.x returns from 0 point in the middle of the tile therefore x2, + additional offset to make sure objects disappear outside of view
        threshhold = GameObject.Find("Street").transform.localScale.x * 2.0f + Length * 4.0f;

        vrInteractableLayer = LayerMask.GetMask("VRInteractables");
        movingObjectLayer = LayerMask.GetMask("Leaving Field");

        detectionDistance = Length / 1.5f;
    }


    void FixedUpdate() {
        CheckForObstacle();

        if (isStopped || GameManager.instance.isPaused) { return; }


        transform.localPosition += moveDirection * (speed * Time.deltaTime);

        DestroyAfterDistance();

    }

    private void CheckForObstacle()
    {
        Vector3 rayDir = -moveDirection;
        

        if (Physics.Raycast(transform.position, rayDir, detectionDistance, movingObjectLayer))
        {
            isStopped = true;
        }
        else if (Physics.Raycast(transform.position, rayDir, out var hit, detectionDistance, vrInteractableLayer))
        {
            isStopped = true;
        }
        else
        {
            isStopped = false;
        }
    }


    private void OnCollisionStay(Collision other)
    {
        Renderer rend = GetComponent<Renderer>();
        
        if(other.gameObject.CompareTag("Barrier"))
        {
            
            rend.enabled = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Barrier"))
        {
            
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected() {
        Vector3 rayDir = -moveDirection;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rayDir * detectionDistance);
    }

    // Safetynet to catch objects in preload tiles or if barrier collision fails 
    void DestroyAfterDistance()
    {
        var objectPosition = transform.position;
        var spawnerPosition = GetComponentInParent<MovingObjectSpawner>().transform.position;
        
        var heading = objectPosition - spawnerPosition;
        
        if (heading.sqrMagnitude > 0.45f)
        {
            Destroy(gameObject);
        }
    }
}
