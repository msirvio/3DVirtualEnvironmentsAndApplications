using UnityEngine;
using UnityEngine.InputSystem;

public class GolfHandler : MonoBehaviour
{
    AudioSource collisionAudio;
    public GameObject hitAudioObject;
    AudioSource hitAudio;
    public Transform camera;
    private LineRenderer lineRenderer;
    Rigidbody rigidbody;
    public float shotPower = 10.0f;
    public float drag = 0.99f;
    public float dragThreshold = 2.0f;

    public float stopSpeed = 0.01f;
    public float collisionBump = 2.0f;
    public float sensivity = 1.0f;
    Vector2 lookInput;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    public GameObject playerObject;
    PlayerController player;
    bool readyToHit = false;
    Vector3 velocityLastFrame = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        player = playerObject.GetComponent<PlayerController>();
        collisionAudio = gameObject.GetComponent<AudioSource>();
        hitAudio = hitAudioObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Drag
        if (rigidbody.linearVelocity != Vector3.zero
            && rigidbody.linearVelocity.magnitude < dragThreshold) 
        {
            readyToHit = false;

            rigidbody.linearVelocity = drag * rigidbody.linearVelocity;

            if (rigidbody.linearVelocity.magnitude < stopSpeed) 
            {
                rigidbody.linearVelocity = Vector3.zero;
            }
        }

        if (rigidbody.linearVelocity == Vector3.zero
            && velocityLastFrame == Vector3.zero) {
            readyToHit = true;
        }

        if (player.GetIsGolfing()) {

            if (Gamepad.current != null)
            {
                lookInput = Gamepad.current.rightStick.ReadValue();
            }
            else
            {
                lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
        
            float inputX = lookInput.x;
            float inputY = lookInput.y;
        
            rotationY += sensivity * inputX;
            rotationX -= sensivity * inputY;
            rotationX = Mathf.Clamp(rotationX, -80.0f, 80.0f);

            camera.eulerAngles = new Vector3(rotationX, rotationY, 0);
            camera.position = transform.position;

            Vector3 direction = new Vector3(
                Input.GetAxis("Horizontal"), 
                0, 
                Input.GetAxis("Vertical")
            );

            float angle = Vector3.Angle(direction, camera.forward);
            direction = Quaternion.Euler(0, camera.rotation.eulerAngles.y, 0) * direction;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + direction);
            lineRenderer.material.SetColor("_BaseColor", new Color(1.0f, (1.0f - direction.magnitude), 0.0f, 1.0f));

            player.SetPower(direction.magnitude);



            if (rigidbody.linearVelocity == Vector3.zero && direction != Vector3.zero && readyToHit) {
                player.transform.position = 
                    transform.position 
                    + Quaternion.AngleAxis(90, Vector3.up) * direction.normalized
                    + new Vector3(0,1,0);
                
                Vector3 newDirection = Quaternion.AngleAxis(-90, Vector3.up) * direction.normalized;
                player.transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            }

            if ((Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                || Input.GetKeyDown(KeyCode.F)) {

                readyToHit = false;
                player.SetPower(0);

                if (rigidbody.linearVelocity == Vector3.zero) {
                    rigidbody.AddForce(direction * shotPower);
                    hitAudio.Play();
                }
            }
        }
        velocityLastFrame = rigidbody.linearVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Vector3 normal = collision.contacts[0].normal;

            Vector3 newDirection = Vector3.Reflect(collision.relativeVelocity * (-1), normal);

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.AddForce(newDirection * collisionBump);
            
            collisionAudio.Play();
        }
    }
}
