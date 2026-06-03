using UnityEngine;
using VRCourse.Interaction;

public class Throwable : SimpleVRInteractble
{
    [Header("Throw Force Settings")]
    [SerializeField] private float throwForce = 12f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip throwSFX;

    private Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnVRActivate(SimpleVRInteractorContext context)
    {
        base.OnVRActivate(context);

        if (rb != null)
        {
            // Detach from controller to let physics take over
            transform.SetParent(null);
            rb.isKinematic = false;

            // Determine throw direction based on hand orientation
            Vector3 throwDirection = (context != null && context.Transform != null) 
                ? context.Transform.forward 
                : transform.forward;

            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            if (throwSFX != null)
            {
                PlaySFX(throwSFX);
            }
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}