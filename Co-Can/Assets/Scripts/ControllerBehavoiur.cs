using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ControllerBehavoiur : MonoBehaviour
{
    [SerializeField] private PlayerCtrl player;
    [SerializeField] private Transform target;
    [SerializeField] private EventTrigger controlButton;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] private Camera cam;
    [SerializeField] private float distance;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float defaultAngle;

    private RectTransform buttonRect;
    private float nowAngle;
    private bool isPresed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null) target = player.transform;
        if (cam == null) cam = Camera.main;
        nowAngle = defaultAngle;
        isPresed = false;
        if (controlButton != null)
        {
            buttonRect = controlButton.gameObject.GetComponent<RectTransform>();
            SetTrigger(EventTriggerType.PointerDown, () => SetPressed(true));
            SetTrigger(EventTriggerType.PointerUp, () => SetPressed(false));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPresed)
        {
            ChaseMousePosition();
        }
        SetPosition();
    }

    private void SetPosition()
    {
        SetPlayerDirection();
        Vector2 targetPos = TragetPositionToRectangle();
        Vector2 difference = DegreeToDirection(nowAngle) * distance;

        buttonRect.anchoredPosition = targetPos + difference;
        buttonRect.localEulerAngles = Vector3.forward * nowAngle;
    }

    private Vector2 DegreeToDirection(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return Vector2.right * Mathf.Cos(rad) + Vector2.up * Mathf.Sin(rad);
    }

    private Vector2 TragetPositionToRectangle()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(target.position);
        Vector2 canvasPos = ScreenPointToLocalPointInRectangle(screenPos);
        return canvasPos + offset;
    }

    private void ChaseMousePosition()
    {
        Vector2 mousePos = ScreenPointToLocalPointInRectangle(Input.mousePosition);
        Vector2 difference = mousePos - TragetPositionToRectangle();
        nowAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
    }

    private Vector2 ScreenPointToLocalPointInRectangle(Vector2 screenPos)
    {
        Vector2 canvasPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out canvasPos);
        return canvasPos;
    }

    public void SetPressed(bool _isPressed)
    {
        isPresed = _isPressed;
        player.SetMoving(_isPressed);
    }

    private void SetPlayerDirection()
    {
        float degree = cam.transform.eulerAngles.y - nowAngle + 90;
        if (degree < -180) degree += 360;
        if (degree > 180) degree -= 360;
        player.transform.localEulerAngles = Vector3.up * degree;
    }

    private void SetTrigger(EventTriggerType type, UnityAction action)
    {
        if (controlButton == null) return;
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((x) => action.Invoke());
        controlButton.triggers.Add(entry);
    }
}
