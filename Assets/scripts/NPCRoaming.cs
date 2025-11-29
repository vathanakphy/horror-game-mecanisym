using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCRoaming : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform model;
    public Transform player;
    public Transform cameraScarePoint; // Empty in front of ghost

    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Settings")]
    public float waypointThreshold = 1f;
    public float turnSpeed = 5f;
    public Vector3 modelForwardOffset = Vector3.zero;

    [Header("Player Detection")]
    public float detectionDistance = 15f;
    public float maxFollowDistance = 20f;

    [Header("Close Range Scare Detection")]
    public float scareDistance = 2f;
    public bool alreadyTriggeredScare = false;

    [Header("Flying Settings")]
    public float hoverHeight = 0.2f;
    public float hoverAmplitude = 0.8f;
    public float hoverFrequency = 1f;

    public bool followingPlayer = false;
    private List<Transform> originalWaypoints = new List<Transform>();
    private int currentWaypointIndex = -1;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        modelForwardOffset = new Vector3(-90f, 0f, 90f);
        model.localRotation = Quaternion.Euler(modelForwardOffset);
        originalWaypoints.AddRange(waypoints);

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        GoToNextWaypoint();
    }

    void Update()
    {
        if (player != null)
        {
            if (HideBox.IsPlayerHiddenAnywhere())
            {
                followingPlayer = false;
            }
            else
            {
                Vector3 playerPosFlat = new Vector3(player.position.x, transform.position.y, player.position.z);
                Vector3 npcPosFlat = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                float distance = Vector3.Distance(npcPosFlat, playerPosFlat);

                followingPlayer = distance <= detectionDistance && distance <= maxFollowDistance;
            }
        }

        if (followingPlayer && player != null)
        {
            agent.SetDestination(player.position);
        }
        else if (!agent.pathPending && agent.remainingDistance < waypointThreshold)
        {
            GoToNextWaypoint();
        }

        CheckCloseDetection();
        HoverMotion();
        RotateBody();
        RotateModel();
    }


    private void CheckCloseDetection()
    {
        if (player == null || alreadyTriggeredScare) return;

        // Stop scary behavior if player is hidden
        if (HideBox.IsPlayerHiddenAnywhere()) return;

        Vector3 playerPosFlat = new Vector3(player.position.x, transform.position.y, player.position.z);
        Vector3 npcPosFlat = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float dist = Vector3.Distance(npcPosFlat, playerPosFlat);

        if (dist <= scareDistance)
        {
            alreadyTriggeredScare = true;
            agent.isStopped = true;
            transform.LookAt(player);

            GhostCameraController.Instance.MoveCameraToPoint(cameraScarePoint, 1f);
        }
    }


    private void GoToNextWaypoint()
    {
        if (originalWaypoints.Count == 0) return;
        currentWaypointIndex = Random.Range(0, originalWaypoints.Count);
        agent.SetDestination(originalWaypoints[currentWaypointIndex].position);
    }

    private void RotateBody()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0;
        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void RotateModel()
    {
        if (model != null)
            model.localRotation = Quaternion.Euler(modelForwardOffset);
    }

    private void HoverMotion()
    {
        float baseY = agent.nextPosition.y;
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        float minHover = 0.1f;
        float maxHover = 0.2f;
        float finalHover = Mathf.Clamp(hoverHeight + hoverOffset, minHover, maxHover);
        Vector3 pos = transform.position;
        pos.y = baseY + finalHover;
        transform.position = pos;
    }
}
