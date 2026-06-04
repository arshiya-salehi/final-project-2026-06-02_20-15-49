using UnityEngine;

public class CatchableFish : MonoBehaviour
{
    private FishSwim _fishSwim;
    private Rigidbody _rb;
    private Collider _collider;

    private void Awake()
    {
        _fishSwim = GetComponent<FishSwim>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    public void Catch(Transform attachPoint)
    {
        if (_fishSwim != null) _fishSwim.enabled = false;

        // Disable physics if it has any, but we want it to follow the hook
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        // Parent to the (unscaled) attach point so the fish keeps its own scale.
        transform.SetParent(attachPoint, true);

        // Head-up natural catch: the fish forward axis (+Z, its nose) points up
        // toward the hook, body hanging straight down in world space.
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        // Place the fish so its head/mouth sits right at the attach point with no
        // visible gap. We hang it down by half its world height (long axis bounds).
        float halfHeight = GetHalfHeight();
        transform.position = attachPoint.position - Vector3.up * halfHeight;

        // Optionally disable collider so it doesn't interfere with other fish
        if (_collider != null) _collider.enabled = false;
    }

    private float GetHalfHeight()
    {
        // Use renderer world bounds so scale is accounted for. The fish's long
        // (nose-to-tail) axis becomes vertical when hung, so use the largest extent.
        Renderer r = GetComponentInChildren<Renderer>();
        if (r != null)
        {
            Vector3 e = r.bounds.extents;
            return Mathf.Max(e.x, e.y, e.z);
        }
        return 0.25f;
    }

    public void Store(Transform cup)
    {
        // Detach from the hook so it no longer follows it, then deposit in the cup.
        transform.SetParent(null, true);
        gameObject.SetActive(false);
        // Destroy(gameObject); // Or destroy
    }
}
