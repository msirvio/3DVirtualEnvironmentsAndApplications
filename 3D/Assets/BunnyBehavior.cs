using System.Collections.Generic;
using UnityEngine;


public class BunnyBehavior : MonoBehaviour
{
    [Header("Bunny details")]
    public GameObject bunny;
    public float baseSpeed;
    public float escapeSpeed;
    private Collider bunnyCollider;
    private Rigidbody bunnyRigidBody;
    private float waitTimer = 0f;
    private float escapeTimer = 0f;
    private float escapeDuration = 0.8f;


    [Header("Player")]
    public GameObject player;

    [Header("Checkpoint details")]
    public GameObject checkpointsObject;
    public Transform start;

    private Collider nextCheckpointCollider;
    private int checkpointIndex = 0;
    private int checkpointIndexMax;
    private Vector3 direction;
    private List<Collider> checkpointList = new List<Collider>();
    private bool isPlayerNear = false;
    private readonly float bunnyEscapeTreshold = 4f;
    private enum BunnyState {None, Patrol, Escape, Wait};
    private BunnyState currentState = BunnyState.None;


    void Start()
    {
        bunny.transform.SetLocalPositionAndRotation(start.position, start.rotation);

        checkpointList.AddRange(checkpointsObject.GetComponentsInChildren<Collider>());
        checkpointIndexMax = checkpointList.Count;
        Debug.Log("checkpointIndexMax: " + checkpointIndexMax);

        bunnyCollider = bunny.GetComponentInChildren<Collider>();
        bunnyRigidBody = bunny.GetComponent<Rigidbody>();

        Debug.Log("checkpointList value on start: " + checkpointList);
        nextCheckpointCollider = checkpointList[checkpointIndex];
        Debug.Log("nextCheckPoint value on start: " + nextCheckpointCollider);
    }


    void FixedUpdate()
    {
        // Initialize on first frame
        if (currentState == BunnyState.None) {
            currentState = BunnyState.Patrol;
        }

        float bunnyDistanceFromPlayer = Vector3.Distance(bunny.transform.position, player.transform.position);
        bool isBunnyEscaping = bunnyDistanceFromPlayer < bunnyEscapeTreshold;

        if (!isBunnyEscaping && currentState != BunnyState.Wait && escapeTimer > escapeDuration) {
            isPlayerNear = false;
            Debug.Log("Setting enum to PATROL at start of function");
            currentState = BunnyState.Patrol;
        }
        else if (isBunnyEscaping) {
            isPlayerNear = true;
            Debug.Log("Setting enum to ESCAPE at start of function");
            currentState = BunnyState.Escape;
        }

        if (currentState == BunnyState.Escape && !isPlayerNear) {
            escapeTimer += Time.fixedDeltaTime;
            //Debug.Log("escapeTimer updated: " + escapeTimer);
            if (escapeTimer >= escapeDuration)
            {
                Debug.Log("Setting enum to WAIT at start of function");
                currentState = BunnyState.Wait;
                waitTimer = 0f;
                escapeTimer = 0f;
            }
        }

        if (waitTimer >= 2.8f) {
            isPlayerNear = false;
            waitTimer = 0f;
            Debug.Log("Setting enum to PATROL at waitTimer >= 3f");
            currentState = BunnyState.Patrol;
        }

        RaycastHit hit;
        switch (currentState) {
            case BunnyState.Patrol:
                direction = (nextCheckpointCollider.transform.position - bunny.transform.position).normalized;
                //Debug.Log("nextCheckPoint value on FixedUpdate: " + nextCheckpointCollider);
                //Debug.Log("Vector where Bunny should move: " + direction);

                if (Physics.Raycast(bunny.transform.position, Vector3.down, out hit, 2f)) {           
                    direction = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                }
                bunnyRigidBody.linearVelocity = direction * baseSpeed;

                if (direction != Vector3.zero) {
                    Quaternion targetRotationPatrol = Quaternion.LookRotation(direction, hit.normal);
                    bunny.transform.rotation = Quaternion.Slerp(bunny.transform.rotation, targetRotationPatrol, Time.deltaTime * 5f);
                }
                break;

            case BunnyState.Escape:
                direction = -(player.transform.position - bunny.transform.position).normalized;

                if (Physics.Raycast(bunny.transform.position, Vector3.down, out hit, 2f)) {           
                    direction = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                }
                bunnyRigidBody.linearVelocity = direction * escapeSpeed;

                if (direction != Vector3.zero) {
                    Quaternion targetRotationEscape = Quaternion.LookRotation(direction, hit.normal);
                    bunny.transform.rotation = Quaternion.Slerp(bunny.transform.rotation, targetRotationEscape, Time.deltaTime * 15f);
                }
                if (!isBunnyEscaping) {
                    isPlayerNear = false;
                }
                break;

            case BunnyState.Wait:
                bunnyRigidBody.linearVelocity = Vector3.zero;
                waitTimer += Time.fixedDeltaTime;

                direction = (player.transform.position - bunny.transform.position).normalized;

                if (Physics.Raycast(bunny.transform.position, Vector3.down, out hit, 2f)) {           
                    direction = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                }

                Quaternion targetRotationWait = Quaternion.LookRotation(direction, hit.normal);
                bunny.transform.rotation = Quaternion.Slerp(bunny.transform.rotation, targetRotationWait, Time.deltaTime * 25f);

                break;
        }

        // Checkpoint handler
        if (currentState == BunnyState.Patrol) {
            if (bunnyCollider.bounds.Intersects(nextCheckpointCollider.bounds)) {

            checkpointIndex++;
            if (checkpointIndex >= checkpointIndexMax) {
                checkpointIndex = 0;
            }
            nextCheckpointCollider = checkpointList[checkpointIndex];
            }
        }
    }
}
