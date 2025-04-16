using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{ 
    bool isGolfing = false;
    public Transform camera;
    private CharacterController controller;
    public float movementSpeed = 5.0f;
    public float resetHeight = 0.001f;
    Vector3 direction = Vector3.zero;
    AudioSource walkAudio;
    bool audioIsPlaying = false;
   


    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;


    Vector2 lookInput;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public float sensivity = 1.0f;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        walkAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        //Controller supported
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonNorth.wasPressedThisFrame)
            {
                isGolfing = !isGolfing;
            }
        } else {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isGolfing = !isGolfing;
            }
        }

        if (!isGolfing) {
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

            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            direction = new Vector3(
                Input.GetAxis("Horizontal"), 
                0, 
                Input.GetAxis("Vertical")
            );

            float angle = Vector3.Angle(direction, camera.forward);
            direction = Quaternion.Euler(0, camera.rotation.eulerAngles.y, 0) * direction;
        
            controller.Move(direction.normalized * movementSpeed * Time.deltaTime);



            if (groundedPlayer)
            {
                if ((Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) 
                || Input.GetKeyDown(KeyCode.Space))
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                }
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
    

            if (direction != Vector3.zero) 
            {
                gameObject.transform.forward = direction;

                if (!audioIsPlaying) {
                    walkAudio.Play();
                    audioIsPlaying = true;
                }

            } else {
                walkAudio.Stop();
                audioIsPlaying = false;
            }
        }
    }

    public bool GetIsGolfing () {
        return isGolfing;
    }

    public void SetIsGolfing (bool newIsGolfing) {
        isGolfing = newIsGolfing;
    }
}
