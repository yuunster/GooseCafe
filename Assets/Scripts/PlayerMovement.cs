using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider coll;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 collCenter;

    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int MoveState = Animator.StringToHash("Base Layer.move");
    private static readonly int SurprisedState = Animator.StringToHash("Base Layer.surprised");
    private static readonly int AttackState = Animator.StringToHash("Base Layer.attack_shift");
    private static readonly int DissolveState = Animator.StringToHash("Base Layer.dissolve");
    private static readonly int AttackTag = Animator.StringToHash("Attack");
    [SerializeField] private SkinnedMeshRenderer[] MeshR;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float animationTransitionSpeed = 0.1f;
    [SerializeField] private float maxSlopeAngle = 30f;

    public bool grounded { get; private set; }
    public RaycastHit groundedHit;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        collCenter = transform.TransformPoint(coll.center);
    }

    private void Update()
    {
        Movement();
        Animation();
    }

    private void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        grounded = Physics.Raycast(collCenter, Vector3.down, out groundedHit, coll.height + 0.1f); // Check if grounded

        if (grounded)
        {
            float angle = Vector3.Angle(groundedHit.normal, Vector3.up);    // Calculate angle of the floor
            if (angle >= maxSlopeAngle) return;     // Check if angle of ground is too steep to walk
        }

        if (moveDirection.magnitude > 0)
        {
            rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void Animation()
    {
        if (moveDirection.magnitude > 0)
        {
            animator.CrossFade(MoveState, animationTransitionSpeed);
        }
        else
        {
            animator.CrossFade(IdleState, animationTransitionSpeed);
        }
    }
}
