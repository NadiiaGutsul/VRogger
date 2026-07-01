using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask snapGridLayer;
    [SerializeField] private LayerMask borderLayer;

    [Header("Model")]
    [SerializeField] private GameObject playerModel;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Player Movement")]
    Rigidbody rb;
    private float moveDelay = 0;
    private Vector3 moveDir;

    [Header("Respawn")]
    private Vector3 startPosition;
    private Transform startTile;
    private float startHeight;
    [SerializeField] private GameObject pcEnvironment;

    [Header("Debug")]
    private Vector3 worldDirection_RAYCASTTEST;

    public UnityEvent<GameObject> goalReached;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        CalcStartPosition();
        
        // Calculate distance between player and startTile once to use or setting start position
        startHeight = Vector3.Distance(transform.transform.position, startTile.position);
        
    }

    private void CalcStartPosition()
    {
        if (Physics.Raycast(transform.position, -transform.up, out var hit, float.PositiveInfinity))
        {
            startTile = hit.transform;
            SetStart();
        }
    }

    // Has to be called before each death to ensure resizing of the playing field is considered 
    private void SetStart()
    {
        startPosition = startTile.position + new Vector3(0f, startHeight, 0f);
    }

    public void FixedUpdate()
    {
        ResetMoveDelay();
        CheckForBorder();

        // DEBUG
        Debug.DrawRay(transform.position, worldDirection_RAYCASTTEST * 100f, Color.red);
        Debug.DrawRay(transform.position, -transform.up * 100f, Color.red);
        Debug.DrawRay(transform.position, transform.right * 100f, Color.green);
    }

    private void OnPlatformEnter()
    {

        if (Physics.Raycast(transform.position, -transform.up, out var hit, float.PositiveInfinity))
        {
            
            if (hit.transform.gameObject.tag.Equals("MovingPlatform"))
            {
                transform.parent = hit.transform;
            }
            else if (hit.transform.gameObject.tag.Equals("Goal"))
            {
                transform.parent = hit.transform.parent;
                OnGoalReached(hit);
            }
            else
            {
                transform.parent = pcEnvironment.transform;
            }
        }
    }

    // Prevent going through wall while sticking to platform 
    private void CheckForBorder()
    {
        if (!transform.parent && transform.parent.tag.Equals("MovingPlatform"))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.01f, borderLayer);

            if (hits.Length > 0)
            {
                transform.parent = pcEnvironment.transform;
            }
        }

    }

    private void OnGoalReached(RaycastHit hit)
    {
        goalReached.Invoke(hit.transform.gameObject);
        CalcStartPosition();
        
    }

    private void ResetMoveDelay()
    {
        if (moveDelay > 0)
        {
            moveDelay -= Time.fixedDeltaTime;
        }
    }
    
    // Handle player movement input 
    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameManager.instance.isPaused || moveDelay > 0) return;

        Vector2 input = context.ReadValue<Vector2>();

        // Prevent diagonal movement
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            input.y = 0;
        }
        else
        {
            input.x = 0;
        }

        if (input == Vector2.zero) return;

        // Calculate movement direction based on Input --> convert local space to world space to keep rotation
        moveDir = transform.TransformDirection(new Vector3(input.x, 0f, input.y)).normalized;

        RotatePlayerModel(moveDir);

        // DEBUG
        worldDirection_RAYCASTTEST = moveDir;

        // Grid-Movement --> Move player to Grid position in direction of intended movement
        if (Physics.Raycast(transform.position, moveDir, out var hit, float.PositiveInfinity, snapGridLayer))
        {
            transform.position = hit.transform.position;
            moveDelay = 0.155f;
            PlaySound(jumpSound, 0.2f);
            PlayJumpAnimation();
        }

        OnPlatformEnter();
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (!audioSource)
        {
            
            return;
        }
        
        audioSource.PlayOneShot(clip, volume);
    }

    private void PlayJumpAnimation()
    {
        if (!animator)
        {
           
            return;
        }
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            animator.SetTrigger("JumpTrigger");

        }
    }

    private void OnDeath()
    {
        // Short timer to delay movement after death to prevent running into obstacles
        PlaySound(hitSound, 0.5f);
        moveDelay = 0.2f;

        SetStart();
        transform.position = startPosition;
        transform.parent = pcEnvironment.transform;
        GameManager.instance.LifeLost.Invoke();
    }

    private void RotatePlayerModel(Vector3 moveDir)
    {
        // transform.up as 2nd parameter to ensure playing field rotation is kept
        playerModel.transform.rotation = Quaternion.LookRotation(moveDir, transform.up) * Quaternion.Euler(0, 180, 0);
    }

    // Handle car collisions
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Obstacle"))
        {
            OnDeath();
        }

        // Backup in case player phases through wall
        if (other.gameObject.tag.Equals("Barrier"))
        {
            transform.parent = pcEnvironment.transform;
            transform.position = startPosition;
        }
    }

}