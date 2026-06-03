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

    public void Catch(Transform hook)
    {
        if (_fishSwim != null) _fishSwim.enabled = false;
        
        // Disable physics if it has any, but we want it to follow the hook
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        transform.SetParent(hook);
        transform.localPosition = new Vector3(0, -0.2f, 0); // Hang slightly below the hook center
        transform.localRotation = Quaternion.Euler(90, 0, 0); // Rotate to hang vertically

        // Optionally disable collider so it doesn't interfere with other fish
        if (_collider != null) _collider.enabled = false;
    }

    public void Store(Transform cup)
    {
        // Simple storage: just disable or destroy
        // You could also parent it to the cup or play an effect
        gameObject.SetActive(false);
        // Destroy(gameObject); // Or destroy
    }
}
