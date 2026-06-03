using UnityEngine;

public class FishStorage : MonoBehaviour
{
    [SerializeField] private AudioSource storeSound;
    [SerializeField] private ParticleSystem storeEffect;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is a hook with a caught fish
        FishingHook hook = other.GetComponentInParent<FishingHook>() ?? other.GetComponent<FishingHook>();
        if (hook != null && hook.caughtFish != null)
        {
            StoreFish(hook);
        }
        else
        {
            // Or check if the fish itself entered (if it still has a collider or if we use parent check)
            CatchableFish fish = other.GetComponent<CatchableFish>();
            if (fish != null && transform.parent != null && other.transform.parent.GetComponent<FishingHook>() != null)
            {
                 StoreFish(other.transform.parent.GetComponent<FishingHook>());
            }
        }
    }

    private void StoreFish(FishingHook hook)
    {
        if (storeSound != null) storeSound.Play();
        if (storeEffect != null) storeEffect.Play();

        hook.caughtFish.Store(transform);
        hook.ReleaseFish();
    }
}
