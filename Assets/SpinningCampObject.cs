using UnityEngine;
using VRCourse.Interaction;

/// <summary>
/// Attach this script to a primitive sphere or cube in your campsite.
/// Activating it causes it to spin smoothly on its axis.
/// </summary>
public class SpinningCampObject : SimpleVRInteractble
{
    [Header("Spin Settings")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private AudioClip interactiveSFX;

    private bool isSpinning = false;

    protected virtual void Update()
    {
        // If active, rotate around the Y-axis smoothly over time
        if (isSpinning)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    protected override void OnVRActivate(SimpleVRInteractorContext context)
    {
        base.OnVRActivate(context);

        // Toggle the spin state
        isSpinning = !isSpinning;

        // Play the custom spatial interaction sound cue
        if (interactiveSFX != null)
        {
            AudioSource.PlayClipAtPoint(interactiveSFX, transform.position);
        }
    }
}