using UnityEngine;
using VRCourse.Interaction;

/// <summary>
/// Attach this script to your 'Flashlight_On' object.
/// Grabbing the flashlight and clicking the trigger will toggle its spot-light beam on and off.
/// </summary>
public class CampFlashlight : SimpleVRInteractble
{
    [Header("Flashlight Light Reference")]
    [Tooltip("Drag the child Light component (the cone spotlight) here.")]
    [SerializeField] private Light flashlightBeam;

    [Header("Audio Settings")]
    [Tooltip("A mechanical click sound effect when switching the flashlight on/off.")]
    [SerializeField] private AudioClip switchSFX;

    protected virtual void Start()
    {
        // If not assigned, search recursively in child objects for a Light component
        if (flashlightBeam == null)
        {
            flashlightBeam = GetComponentInChildren<Light>();
        }

        // Start with the flashlight off during setup if preferred
        if (flashlightBeam != null)
        {
            flashlightBeam.enabled = false;
        }
    }

    /// <summary>
    /// Runs when the player squeezes the trigger while holding the flashlight.
    /// </summary>
    protected override void OnVRActivate(SimpleVRInteractorContext context)
    {
        base.OnVRActivate(context);

        if (flashlightBeam != null)
        {
            // Toggle the light state (True becomes False, False becomes True)
            flashlightBeam.enabled = !flashlightBeam.enabled;

            // Play a mechanical clicking audio cue spatially
            if (switchSFX != null)
            {
                AudioSource.PlayClipAtPoint(switchSFX, transform.position);
            }
        }
    }
}