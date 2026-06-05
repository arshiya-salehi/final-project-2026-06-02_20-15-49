using UnityEngine;
using UnityEngine.InputSystem;

public class FishingHook : MonoBehaviour
{
    public CatchableFish caughtFish { get; private set; }
    
    [Header("Effects")]
    [SerializeField] private GameObject splashPrefab;

    [Header("Attachment")]
    [SerializeField] private Transform attachPoint;

    [Header("Attraction Settings")]
    [SerializeField] private float attractionRadius = 15f;

    [Header("Stardew Minigame Mechanics")]
    public GameObject minigameUI;
    public GameObject exclamationMark;
    public AudioSource biteAudio;
    public InputActionReference hookSetAction;
    public float biteWindow = 2.0f;

    private float _timer;
    private FishSwim _attractedFish;
    
    private bool isBiting = false;
    private float biteTimer = 0f;
    private CatchableFish pendingFish = null;

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

        if (isBiting)
        {
            biteTimer -= Time.deltaTime;
            
            bool isTriggerPulled = (hookSetAction != null && hookSetAction.action.ReadValue<float>() > 0.5f) || 
                                   (Keyboard.current != null && Keyboard.current.upArrowKey.isPressed);

            if (isTriggerPulled)
            {
                isBiting = false;
                if (exclamationMark != null) exclamationMark.SetActive(false);
                
                if (minigameUI != null)
                {
                    Transform playerCam = Camera.main.transform;
                    Vector3 flatForward = new Vector3(playerCam.forward.x, 0, playerCam.forward.z).normalized;
                    minigameUI.transform.position = playerCam.position + flatForward * 1.5f;
                    minigameUI.transform.LookAt(playerCam);
                    minigameUI.transform.Rotate(0, 180, 0);
                    minigameUI.SetActive(true);
                }
            }
            else if (biteTimer <= 0)
            {
                isBiting = false;
                pendingFish = null;
                if (exclamationMark != null) exclamationMark.SetActive(false);
            }
            
            return;
        }

        _timer += Time.deltaTime;
        FindAndAttractNearestFish();
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

    private void OnTriggerEnter(Collider other)
    {
        if (caughtFish != null || isBiting) return;

        CatchableFish fish = other.GetComponent<CatchableFish>();
        if (fish != null)
        {
            pendingFish = fish;
            isBiting = true;
            biteTimer = biteWindow;

            if (exclamationMark != null) exclamationMark.SetActive(true);
            if (biteAudio != null) biteAudio.Play();
            if (splashPrefab != null) Instantiate(splashPrefab, transform.position, Quaternion.identity);
        }
    }

    public void FinalizeCatch()
    {
        if (pendingFish != null)
        {
            caughtFish = pendingFish;
            caughtFish.Catch(attachPoint != null ? attachPoint : transform);
        }
        if (minigameUI != null) minigameUI.SetActive(false);
        pendingFish = null;
    }

    public void ReleaseFish()
    {
        caughtFish = null;
        pendingFish = null;
    }
}
