using UnityEngine;
using System.Collections;

public class HoleHandler : MonoBehaviour
{
    AudioSource collisionAudio;
    public Transform nextLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisionAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            
            collisionAudio.Play();

            collision.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            collision.gameObject.transform.position = nextLocation.position;

        }
    }
}
