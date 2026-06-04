using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Drives the fishing hook on a scripted "line" that hangs straight down from the
/// rod tip. The line length (and therefore the hook depth) is controlled by the
/// right controller thumbstick: push up to reel in, pull down to let out.
/// This replaces the physics ConfigurableJoint, which mis-behaved on the heavily
/// scaled rod/hook.
/// </summary>
public class HookReel : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The rod tip the line hangs from.")]
    [SerializeField] private Transform tip;
    [Tooltip("The hook driven by this reel.")]
    [SerializeField] private Transform hook;

    [Header("Input")]
    [Tooltip("Vector2 action (thumbstick). Y axis reels in (up) / out (down).")]
    [SerializeField] private InputActionReference reelAction;

    [Header("Line Settings")]
    [Tooltip("Current length of the line in meters (hook depth below the tip).")]
    [SerializeField] private float lineLength = 1.2f;
    [SerializeField] private float minLength = 0.15f;
    [SerializeField] private float maxLength = 3.0f;
    [Tooltip("How fast the line reels in/out, in meters per second.")]
    [SerializeField] private float reelSpeed = 1.5f;
    [Tooltip("Dead zone for the thumbstick Y axis.")]
    [SerializeField] private float deadZone = 0.15f;

    private void OnEnable()
    {
        if (reelAction != null && reelAction.action != null)
            reelAction.action.Enable();
    }

    private void OnDisable()
    {
        if (reelAction != null && reelAction.action != null)
            reelAction.action.Disable();
    }

    private void Update()
    {
        if (reelAction == null || reelAction.action == null) return;

        float y = reelAction.action.ReadValue<Vector2>().y;
        if (Mathf.Abs(y) < deadZone) return;

        // Push up (positive y) reels in -> shorter line.
        lineLength -= y * reelSpeed * Time.deltaTime;
        lineLength = Mathf.Clamp(lineLength, minLength, maxLength);
    }

    private void LateUpdate()
    {
        if (tip == null || hook == null) return;

        // Hook always hangs straight down (world -Y) from the rod tip, at the
        // current line length. Rotation kept upright so a caught fish stays vertical.
        hook.position = tip.position + Vector3.down * lineLength;
        hook.rotation = Quaternion.identity;
    }
}
