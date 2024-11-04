using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Customer : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Image image;
    public Label label;
    public Coroutine patienceTimer;
    public int orderIndex;
    public bool waiting = false;
    public bool leaving = false;
    public Transform leader;
    private bool hasArrived = false;
    private float followDistance = 1.3f;


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        leader = null;
    }

    void Update()
    {
        // Check if the agent has reached the waiting area
        if (waiting && !hasArrived && navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.pathPending)
        {
            hasArrived = true;
        }

        if (hasArrived && !leaving)
        {
            RotateTowardsPlayer();
        }

        if (leader != null)
        {
            FollowCustomer();
        }
    }

    private void RotateTowardsPlayer()
    {
        // Smoothly rotate towards the player
        Vector3 direction = (FindObjectOfType<PlayerMovement>().transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
    }

    private void FollowCustomer()
    {
        // Calculate the target position behind the leader at a set distance
        Vector3 directionToLeader = (transform.position - leader.position).normalized;
        Vector3 targetPosition = leader.position + directionToLeader * followDistance;

        navAgent.SetDestination(targetPosition);
    }
}
