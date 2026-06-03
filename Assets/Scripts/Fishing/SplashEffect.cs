using UnityEngine;

public class SplashEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float destroyDelay = 2.0f;

    void Start()
    {
        if (particles != null) particles.Play();
        if (audioSource != null) audioSource.Play();
        
        Destroy(gameObject, destroyDelay);
    }
}
