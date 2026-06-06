using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FishingMinigame : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty = Difficulty.Medium;

    [Header("UI References")]
    public RectTransform catchArea;
    public RectTransform fishIcon;
    public Slider progressBar;
    
    [Header("Hook Reference")]
    public FishingHook activeHook;

    [Header("Input")]
    public InputActionReference vrTriggerAction;

    [Header("Player Settings")]
    public float upwardForce = 800f;
    public float gravity = 400f;
    private float catchAreaVelocity = 0f;

    [Header("Fish Settings")]
    private float fishTargetY;
    private float timeUntilNextMove;
    private float fishVelocity = 0f;

    private float topBoundary = 300f;
    private float bottomBoundary = -300f;

    void Start()
    {
        if (vrTriggerAction?.action != null) vrTriggerAction.action.Enable();
        
        RectTransform track = catchArea.parent.GetComponent<RectTransform>();
        float halfTrack = track.rect.height / 2f;
        float halfCatchArea = catchArea.rect.height / 2f;
        topBoundary = halfTrack - halfCatchArea;
        bottomBoundary = -halfTrack + halfCatchArea;

        fishTargetY = fishIcon.anchoredPosition.y;
        progressBar.value = 0.3f;
    }

    void OnEnable()
    {
        progressBar.value = 0.3f;
    }

    void Update()
    {
        HandlePlayerInput();
        HandleFishMovement();
        CheckProgress();
    }

    void HandlePlayerInput()
    {
        bool isPressed = false;
        if (vrTriggerAction?.action != null)
        {
            isPressed = vrTriggerAction.action.ReadValue<float>() > 0.5f;
        }

        if (Keyboard.current != null && Keyboard.current.upArrowKey.isPressed)
        {
            isPressed = true;
        }

        if (isPressed)
        {
            catchAreaVelocity += upwardForce * Time.deltaTime;
        }
        else
        {
            catchAreaVelocity -= gravity * Time.deltaTime;
        }

        float newY = catchArea.anchoredPosition.y + (catchAreaVelocity * Time.deltaTime);

        if (newY >= topBoundary) { newY = topBoundary; catchAreaVelocity = 0; }
        if (newY <= bottomBoundary) { newY = bottomBoundary; catchAreaVelocity = 0; }

        catchArea.anchoredPosition = new Vector2(catchArea.anchoredPosition.x, newY);
    }

    void HandleFishMovement()
    {
        timeUntilNextMove -= Time.deltaTime;
        if (timeUntilNextMove <= 0)
        {
            fishTargetY = Random.Range(bottomBoundary, topBoundary);
            switch (currentDifficulty)
            {
                case Difficulty.Easy: timeUntilNextMove = Random.Range(2.0f, 3.5f); break;
                case Difficulty.Medium: timeUntilNextMove = Random.Range(1.2f, 2.0f); break;
                case Difficulty.Hard: timeUntilNextMove = Random.Range(0.5f, 1.0f); break;
            }
        }

        float smoothTime = currentDifficulty == Difficulty.Hard ? 0.2f : 0.6f;
        float newFishY = Mathf.SmoothDamp(fishIcon.anchoredPosition.y, fishTargetY, ref fishVelocity, smoothTime);
        fishIcon.anchoredPosition = new Vector2(fishIcon.anchoredPosition.x, newFishY);
    }

    void CheckProgress()
    {
        float distance = Mathf.Abs(fishIcon.anchoredPosition.y - catchArea.anchoredPosition.y);
        float catchAreaHalfHeight = catchArea.rect.height / 2f;

        if (distance < catchAreaHalfHeight)
        {
            progressBar.value = Mathf.Clamp01(progressBar.value + 0.25f * Time.deltaTime);
            if (progressBar.value >= 1f)
            {
                if (activeHook != null) activeHook.FinalizeCatch();
            }
        }
        else
        {
            progressBar.value = Mathf.Clamp01(progressBar.value - 0.15f * Time.deltaTime);
            if (progressBar.value <= 0f)
            {
                if (activeHook != null) { activeHook.ReleaseFish(); gameObject.SetActive(false); }
            }
        }
    }
}