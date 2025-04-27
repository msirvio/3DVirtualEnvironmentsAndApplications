using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
    }
}
