using UnityEngine;
using VRCourse.Interaction;

/// <summary>
/// Attach this script to your 'Campfire_Obj' object.
/// The user can point at or touch the campfire and press trigger to turn the cozy crackle audio loop on/off.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CampfireAudio : SimpleVRInteractble
{
    [Header("Audio Settings")]
    [Tooltip("Drag your looping campfire crackling sound effect clip here.")]
    [SerializeField] private AudioClip crackleSFX;

    [Tooltip("Feedback sound effect when toggling the fire.")]
    [SerializeField] private AudioClip triggerBeepSFX;

    private AudioSource campfireAudioSource;

    protected virtual void Start()
    {
        campfireAudioSource = GetComponent<AudioSource>();

        // Ensure the AudioSource is set up for cozy spatial 3D VR sound
        campfireAudioSource.clip = crackleSFX;
        campfireAudioSource.loop = true;
        campfireAudioSource.spatialBlend = 1.0f; // 1.0 = Fully 3D sound
        campfireAudioSource.playOnAwake = true; // Start playing immediately
        campfireAudioSource.minDistance = 1f;
        campfireAudioSource.maxDistance = 15f;

        if (crackleSFX != null)
        {
            campfireAudioSource.Play();
        }
    }

    /// <summary>
    /// Runs when the player points at/selects the campfire and pulls the trigger.
    /// </summary>
    protected override void OnVRActivate(SimpleVRInteractorContext context)
    {
        base.OnVRActivate(context);

        if (campfireAudioSource != null)
        {
            // Play a spatialized activation beep
            if (triggerBeepSFX != null)
            {
                AudioSource.PlayClipAtPoint(triggerBeepSFX, transform.position);
            }

            // Toggle play/mute state of the campfire sounds
            if (campfireAudioSource.isPlaying)
            {
                campfireAudioSource.Pause();
            }
            else
            {
                campfireAudioSource.Play();
            }
        }
    }
}