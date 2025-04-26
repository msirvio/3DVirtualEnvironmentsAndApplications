using UnityEngine;

public class CatLogic : MonoBehaviour
{
    public Transform player;
    public float moveDistance = 0.05f;
    private float followDistance = 3.0f;
    private float groundLevel = 0.5f;
    private float xRotation = 0.0f;

    private bool isWalking = false;
    private Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move and Rotate
        if (Vector3.Distance(transform.position, player.position) > followDistance) {
            
            isWalking = true;
            
            //Move
            Vector3 newPosition;
            newPosition = Vector3.MoveTowards(transform.position, player.position, moveDistance);
            newPosition.y = groundLevel;
            transform.position = newPosition;

            //Rotate
            Vector3 newDirection = transform.position - player.position;
            transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            transform.eulerAngles = new Vector3(xRotation, transform.eulerAngles.y, transform.eulerAngles.z);
        } else {
            isWalking = false;
        }

        //Animation
        animator.SetBool("IsWalking", isWalking);

    }
}
