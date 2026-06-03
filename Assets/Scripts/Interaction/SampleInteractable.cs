using UnityEngine;
using VRCourse.Interaction;

public class SampleInteractable : SimpleVRInteractble
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;

    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }

    protected override void OnVRHoverEnter(SimpleVRInteractorContext context)
    {
        SetColor(hoverColor);
    }

    protected override void OnVRHoverExit(SimpleVRInteractorContext context)
    {
        SetColor(originalColor);
    }

    protected override void OnVRSelectEnter(SimpleVRInteractorContext context)
    {
        SetColor(selectedColor);
    }

    protected override void OnVRSelectExit(SimpleVRInteractorContext context)
    {
        SetColor(hoverColor);
    }

    protected override void OnVRActivate(SimpleVRInteractorContext context)
    {
        Debug.Log($"{name} activated by {context.GameObject?.name}");
    }

    private void SetColor(Color color)
    {
        if (targetRenderer != null)
            targetRenderer.material.color = color;
    }
}
