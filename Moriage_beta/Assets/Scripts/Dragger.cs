using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class Dragger : Selectable
{
    [SerializeField] private UnityEvent onDragging;
    [SerializeField] private UnityEvent onStartDrag;
    [SerializeField] private UnityEvent onEndDrag;
    private RectTransform canvasRect;
    private Camera cam;
    private RectTransform rectTransform;
    private Vector3 startOffset;
    private bool isDragging = false;
    private Vector3 updatedPosition = new();

    public bool freezeX { get; set; }
    public bool freezeY { get; set; }
    public Vector3 UpdatedPosition => updatedPosition;
    public UnityEvent OnDragging => onDragging;
    public UnityEvent OnStartDrag => onStartDrag;
    public UnityEvent OnEndDrag => onEndDrag;

    private Vector3 MousePosition
    {
        get
        {
            if (canvasRect == null)
            {
                Graphic graphic = GetComponent<Graphic>();
                Canvas canvas = graphic.canvas;
                if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    cam = canvas.worldCamera;
                }
                canvasRect = canvas.GetComponent<RectTransform>();
            }
            Vector3 mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                mousePos,
                cam,
                out Vector2 result);
            return result;
        }
    }

    protected override void Start()
    {
        base.Start();
        rectTransform = GetComponent<RectTransform>();
        startOffset = rectTransform.position - MousePosition;
        freezeX = false;
        freezeY = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            updatedPosition = MousePosition + startOffset;
            rectTransform.position = new(
                freezeX ? rectTransform.position.x : updatedPosition.x,
                freezeY ? rectTransform.position.y : updatedPosition.y,
                rectTransform.position.z
            );
            onDragging?.Invoke();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isDragging = true;
        startOffset = rectTransform.position - MousePosition;
        onStartDrag?.Invoke();
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isDragging = false;
        onEndDrag?.Invoke();
    }
}