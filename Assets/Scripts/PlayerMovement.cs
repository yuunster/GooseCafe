using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider coll;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Vector3 collCenter => coll != null ? transform.TransformPoint(coll.center) : Vector3.zero;
    private GameObject heldItem;
    private Collider[] hits;
    private LayerMask interactablesLayer;

    [SerializeField] private GameObject neck;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float maxSlopeAngle = 30f;
    [SerializeField] private float maxInteractRange = 0.5f;
    [SerializeField] private float carryItemDistance = 0.45f;
    [SerializeField] private float throwSpeed = 4f;

    public bool grounded { get; private set; }
    public RaycastHit groundedHit;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        hits = new Collider[10];
        interactablesLayer = LayerMask.GetMask("Item", "Combiner", "ItemSpawner", "Stove", "Pot", "TrashCan");
    }

    private void Update()
    {
        Movement();
        Interaction();
        Throw();
    }

    private void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        float velocityY = rb.velocity.y > 0 ? 0 : rb.velocity.y;

        grounded = Physics.Raycast(collCenter, Vector3.down, out groundedHit, coll.height + 0.1f); // Check if grounded

        if (grounded)
        {
            float angle = Vector3.Angle(groundedHit.normal, Vector3.up);    // Calculate angle of the floor
            if (angle >= maxSlopeAngle) return;     // Check if angle of ground is too steep to walk
        }

        if (moveDirection.magnitude > 0)
        {
            animator.SetBool("IsMoving", true);

            rb.velocity = moveDirection * moveSpeed + new Vector3(0, velocityY, 0);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);

            rb.velocity = new Vector3(0, velocityY, 0);
        }
    }

    // Visualize BoxCast that handles interaction
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collCenter + new Vector3(0, 0.5f, 0) + transform.forward * maxInteractRange, new Vector3(0.5f, 2f, 0.5f));
    }

    private void Interaction()
    {
        bool isInteracting = Input.GetKeyDown(KeyCode.E);
        if (!isInteracting) return;

        animator.SetTrigger("Interact");

        int numHits = Physics.OverlapBoxNonAlloc(collCenter + new Vector3(0, 0.5f, 0) + transform.forward * maxInteractRange, new Vector3(0.5f, 2f, 0.7f), hits, Quaternion.LookRotation(transform.forward), interactablesLayer);
        if (numHits == 0)
        {
            if (heldItem != null) ReleaseItem(heldItem);
            return;
        }

        // Make a copy of hits array with only numHits as the length. Sort this array by closest -> farthest distance. Return the closest hit.
        Collider closestHit = hits.Take(numHits).OrderBy(hit => Vector3.Distance(transform.position + transform.forward * maxInteractRange, hit.transform.position)).ToArray()[0];
        GameObject closestGO = closestHit.transform.gameObject;

        if (closestGO.layer == LayerMask.NameToLayer("Item"))
        {
            if (heldItem != null)   // If holding something already, drop it.
            {
                ReleaseItem(heldItem);
                return;
            }

            GameObject item = closestHit.transform.gameObject;
            HoldItem(item);
        }
        else if (closestGO.layer == LayerMask.NameToLayer("Combiner"))
        {
            Combiner combinerScript = closestGO.GetComponent<Combiner>();

            if (heldItem == null && combinerScript.outputtedItem != null)   // If not holding something and combiner has an item ready, take it.
            {
                HoldItem(combinerScript.outputtedItem);
            }
            else if (heldItem != null && closestGO.GetComponent<Combiner>().Input(heldItem)) // If holding something, try putting it in the combiner
            {
                heldItem.transform.position = closestGO.transform.position + new Vector3(0, heldItem.GetComponent<BoxCollider>().size.y / 2, 0);
                heldItem.transform.SetParent(closestGO.transform);

                heldItem = null;
            }
        }
        else if (closestGO.layer == LayerMask.NameToLayer("ItemSpawner"))
        {
            if (heldItem != null) return;

            ItemSpawner spawnerScript = closestGO.GetComponent<ItemSpawner>();
            HoldItem(Instantiate(spawnerScript.item));
        }
        else if (closestGO.layer == LayerMask.NameToLayer("Stove"))
        {
            Stove stoveScript = closestGO.GetComponent<Stove>();

            if (heldItem == null && stoveScript.heldItem != null)
            {
                HoldItem(stoveScript.heldItem);
                stoveScript.RemovePot();
            }
            else if (heldItem != null)
            {
                if (heldItem.layer == LayerMask.NameToLayer("Pot") && stoveScript.heldItem == null)     // If holding a pot and stove is empty, put it on the stove
                {
                    stoveScript.Input(heldItem);
                    heldItem = null;
                }
                else if (stoveScript.heldItem != null)      // If holding an item and a pot is on the stove, put item in pot
                {
                    stoveScript.heldItem.GetComponent<Pot>().Input(heldItem);
                    stoveScript.StartCooking();
                    heldItem.transform.position = stoveScript.heldItem.transform.position + new Vector3(0, stoveScript.heldItem.GetComponent<BoxCollider>().size.y / 2, 0);
                    heldItem.transform.SetParent(stoveScript.heldItem.transform);

                    heldItem = null;
                }
                else
                {
                    ReleaseItem(heldItem);
                }
            }
        }
        else if (closestGO.layer == LayerMask.NameToLayer("Pot"))
        {
            Pot potScript = closestGO.GetComponent<Pot>();

            if (heldItem == null)   // If not holding something take the pot
            {
                HoldItem(closestGO);
                return;
            }

            if (heldItem.GetComponent<Drink>() && closestGO.CompareTag("CookedBobaPot"))   // If holding a drink & CookedBobaPot, use boba
            {
                CookedBobaPot cookedBobaPotScript = closestGO.GetComponent<CookedBobaPot>();
                Drink drinkScript = heldItem.GetComponent<Drink>();

                cookedBobaPotScript.UseBoba();  // Decreases CookedBobaPot uses by 1
                Destroy(heldItem);
                HoldItem(Instantiate(drinkScript.drinkWithBoba));    // Creates a new drink object with boba added
            }
            else if (heldItem != null) // If holding something, put it in the pot
            {
                potScript.Input(heldItem);
                heldItem.transform.position = closestGO.transform.position + new Vector3(0, closestGO.GetComponent<BoxCollider>().size.y / 2, 0);
                heldItem.transform.SetParent(closestGO.transform);

                heldItem = null;
            }
        }
        else if (closestGO.layer == LayerMask.NameToLayer("TrashCan"))
        {
            if (heldItem == null) return;

            if (heldItem.layer == LayerMask.NameToLayer("Pot"))
            {
                Destroy(heldItem);
                HoldItem(Instantiate(GameAssets.i.pot));
            }
            else
            {
                Destroy(heldItem);
                heldItem = null;
            }
        }
    }

    private void HoldItem(GameObject item)
    {
        heldItem = item;
        heldItem.GetComponent<Collider>().enabled = false;
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
        heldItem.transform.SetParent(neck.transform);
        heldItem.transform.position = neck.transform.position + transform.forward * carryItemDistance - transform.up * heldItem.GetComponent<BoxCollider>().size.y / 2;
        heldItem.transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    private void ReleaseItem(GameObject item)
    {
        heldItem.transform.SetParent(null);
        heldItem.GetComponent<Collider>().enabled = true;
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        heldItem = null;
    }

    private void Throw()
    {
        bool isThrowing = Input.GetButtonDown("Fire1");
        if (!isThrowing) return;

        animator.SetTrigger("Throw");

        if (heldItem == null) return;

        heldItem.transform.SetParent(null);
        heldItem.GetComponent<Collider>().enabled = true;
        Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
        itemRb.isKinematic = false;
        itemRb.velocity = transform.forward * throwSpeed    // Horizontal throw speed
            + transform.up * throwSpeed / 2     // Vertical throw speed 
            + transform.forward * Vector3.Dot(transform.forward, moveDirection) * throwSpeed;   // Additional throw speed if player is moving
        heldItem = null;
    }
}
