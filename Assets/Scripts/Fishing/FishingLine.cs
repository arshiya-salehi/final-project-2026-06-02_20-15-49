using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private Transform hook;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    private void LateUpdate()
    {
        if (tip != null && hook != null)
        {
            _lineRenderer.SetPosition(0, tip.position);
            _lineRenderer.SetPosition(1, hook.position);
        }
    }

    public void Setup(Transform tipTransform, Transform hookTransform)
    {
        tip = tipTransform;
        hook = hookTransform;
    }
}
