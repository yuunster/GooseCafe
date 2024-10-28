using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider coll;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 collCenter => transform.TransformPoint(coll.center);
    private GameObject heldItem;

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
    [SerializeField] private float maxInteractRange = 2f;
    [SerializeField] private float carryItemDistance = 0.5f;

    public bool grounded { get; private set; }
    public RaycastHit groundedHit;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        Movement();
        Interaction();
        Animation();
    }

    private void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        float velocityY = rb.velocity.y > 0 ? 0 : rb.velocity.y;

        grounded = Physics.Raycast(collCenter, Vector3.down, out groundedHit, coll.height + 0.1f); // Check if grounded

        if (grounded)
        {
            float angle = Vector3.Angle(groundedHit.normal, Vector3.up);    // Calculate angle of the floor
            if (angle >= maxSlopeAngle) return;     // Check if angle of ground is too steep to walk
        }

        if (moveDirection.magnitude > 0)
        {
            rb.velocity = moveDirection * moveSpeed + new Vector3(0, velocityY, 0);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, velocityY, 0);
        }
    }

    private void Interaction()
    {
        Vector3 startRaycastPos = collCenter - new Vector3(0, (coll.height / 2) - 0.1f, 0);
        Debug.DrawLine(startRaycastPos, startRaycastPos + transform.forward * (coll.radius + maxInteractRange), Color.red);

        bool isInteracting = Input.GetKeyDown(KeyCode.E);
        if (!isInteracting) return;

        bool isWithinRange = Physics.Raycast(startRaycastPos, transform.forward, out RaycastHit hitInfo, coll.radius + maxInteractRange);

        if (!isWithinRange) return;

        int itemLayer = LayerMask.NameToLayer("Item");
        int combinerLayer = LayerMask.NameToLayer("Combiner");

        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (heldItem is not null) return;

            GameObject item = hitInfo.transform.gameObject;
            HoldItem(item);
        } else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Combiner"))
        {
            GameObject combiner = hitInfo.transform.gameObject;
            Combiner combinerScript = combiner.GetComponent<Combiner>();

            if (heldItem is null && combinerScript.outputtedItem is not null)
            {
                HoldItem(combinerScript.outputtedItem);
            }
            else if(heldItem is not null && combiner.GetComponent<Combiner>().Input(heldItem))
            {
                heldItem.transform.position = combiner.transform.position + new Vector3(0, combiner.GetComponent<BoxCollider>().size.y / 2, 0);
                heldItem.transform.SetParent(combiner.transform);

                heldItem = null;
            }
        }
    }

    private void HoldItem(GameObject item)
    {
        heldItem = item;
        heldItem.transform.position = transform.position +
            (transform.forward.normalized * carryItemDistance) +
            new Vector3(0, heldItem.GetComponent<BoxCollider>().size.y / 2, 0);
        heldItem.transform.SetParent(this.transform);
        heldItem.GetComponent<Collider>().enabled = false;
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
        heldItem.transform.rotation = Quaternion.identity;
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
