using UnityEngine;

public class FishingHook : MonoBehaviour
{
    public CatchableFish caughtFish { get; private set; }
    
    [Header("Effects")]
    [SerializeField] private GameObject splashPrefab;

    [Header("Attachment")]
    [Tooltip("Unscaled child the caught fish is attached to (keeps fish at correct scale).")]
    [SerializeField] private Transform attachPoint;

    [Header("Attraction Settings")]
    [SerializeField] private float attractionRadius = 15f;

    [Header("Debug")]
    [Tooltip("If enabled, force-catches the nearest fish on a timer (testing only).")]
    [SerializeField] private bool debugAutoCatch = false;
    [SerializeField] private float forceCatchInterval = 10f;

    private float _timer;
    private FishSwim _attractedFish;

    private void Update()
    {
        if (caughtFish != null)
        {
            _timer = 0;
            if (_attractedFish != null)
            {
                _attractedFish.SetForcedTarget(null);
                _attractedFish = null;
            }
            return;
        }

        _timer += Time.deltaTime;

        // Attract the nearest fish
        FindAndAttractNearestFish();

        // Force a catch on a timer (testing only)
        if (debugAutoCatch && _timer >= forceCatchInterval)
        {
            ForceCatch();
            _timer = 0;
        }
    }

    private void FindAndAttractNearestFish()
    {
        CatchableFish[] allFish = Object.FindObjectsByType<CatchableFish>(FindObjectsSortMode.None);
        float minDistance = float.MaxValue;
        CatchableFish nearest = null;

        foreach (var fish in allFish)
        {
            if (fish.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, fish.transform.position);
                if (dist < minDistance && dist < attractionRadius)
                {
                    minDistance = dist;
                    nearest = fish;
                }
            }
        }

        if (nearest != null)
        {
            FishSwim swim = nearest.GetComponent<FishSwim>();
            if (swim != null)
            {
                if (_attractedFish != null && _attractedFish != swim)
                {
                    _attractedFish.SetForcedTarget(null);
                }
                _attractedFish = swim;
                _attractedFish.SetForcedTarget(transform);
            }
        }
    }

    private void ForceCatch()
    {
        if (caughtFish != null) return;

        CatchableFish[] allFish = Object.FindObjectsByType<CatchableFish>(FindObjectsSortMode.None);
        float minDistance = float.MaxValue;
        CatchableFish nearest = null;

        foreach (var fish in allFish)
        {
            if (fish.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, fish.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = fish;
                }
            }
        }

        if (nearest != null)
        {
            // Move the fish to the hook instantly to trigger the catch
            nearest.transform.position = transform.position;
            // The OnTriggerEnter will handle the rest
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (caughtFish != null) return;

        CatchableFish fish = other.GetComponent<CatchableFish>();
        if (fish != null)
        {
            caughtFish = fish;
            fish.Catch(attachPoint != null ? attachPoint : transform);
            
            // Trigger splash effect
            if (splashPrefab != null)
            {
                Instantiate(splashPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    public void ReleaseFish()
    {
        caughtFish = null;
    }
}
