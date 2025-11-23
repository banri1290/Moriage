using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RoundCamera : GameSystem
{
    enum RotateMode { Scrollbar, Buttons }

    public class ChangeRotateEvent : UnityEvent<float> { }

    [Header("UI参照")]
    [Tooltip("カメラの回転を制御するUIスクロールバー")]
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private bool scrollbarActive;
    [Header("カメラ設定")]
    [Tooltip("カメラの回転中心からの距離")]
    [SerializeField] private float radius = 5.0f;
    [Tooltip("カメラの高さ")]
    [SerializeField] private float height = 2.0f;
    [Tooltip("カメラの注視点のオフセット")]
    [SerializeField] private float aimOffset = 1.0f;
    [Tooltip("スクロールバーの操作を反転させるか")]
    [SerializeField] private bool invertScroll = false;
    [Tooltip("自動回転の速度")]
    [SerializeField] private float rotateSpeed = 20.0f;

    private ChangeRotateEvent changeRotateEvent = new();
    private Camera cam;

    private bool isTurning = true;
    private bool isTurningRight = false;

    public ChangeRotateEvent ChangeRotate => changeRotateEvent;
    public float Radius => radius;
    public float Height => height;
    public float AimOffset => aimOffset;
    public float RotateSpeed => rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MouseWheelDrag();
    }

    private void MouseWheelDrag()
    {
        if (isTurning)
        {
            float mouseWheelSensitivity = Input.mouseScrollDelta.y * rotateSpeed;
            Turn(mouseWheelSensitivity);
        }
    }

    public override bool CheckSettings()
    {
        if (radius <= 0)
        {
            Debug.LogWarning("Radius must be greater than zero.");
            radius = 1e-3f;
        }
        bool AllSettingsAreCorrect = true;

        if (scrollbar == null)
        {
            Debug.LogError("Scrollbar not assigned.");
            AllSettingsAreCorrect = false;
        }
        cam = GetComponentInChildren<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera not assigned.");
            AllSettingsAreCorrect = false;
        }

        return AllSettingsAreCorrect;
    }

    public void Init()
    {
        isTurning = !scrollbarActive;
        isTurningRight = false;

        SetCamera();
        InitRotateOperator();

        Debug.Log("RoundCamera settings are correct.");
    }

    private void SetTurning(bool turning)
    {
        isTurning = turning;
        Debug.Log("isTurning: " + isTurning);
    }

    private void InitRotateOperator()
    {
        if (scrollbar != null)
        {
            scrollbar.gameObject.SetActive(scrollbarActive);
        }
        scrollbar.onValueChanged.AddListener(RotateCamera);
        SetScrollbarWithCurrentAngle();
    }

    private void AddTurnEvent2Scrollbar()
    {
        if (scrollbar.GetComponent<EventTrigger>() == null)
        {
            scrollbar.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger trigger = scrollbar.GetComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { SetTurning(false); });
        trigger.triggers.Add(entryPointerDown);
        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((data) => { SetTurning(true); });
        trigger.triggers.Add(entryPointerUp);
    }

    private void SetCamera()
    {
        cam.transform.localPosition = new Vector3(0, height, -radius);

        cam.transform.LookAt(transform.position - transform.forward * aimOffset);
    }

    private void RotateCamera(float value)
    {
        float angle = (value - 0.5f) * 360.0f;
        if (invertScroll) angle *= -1;
        transform.rotation = Quaternion.Euler(0, angle, 0);
        changeRotateEvent.Invoke(angle);
    }

    private void SetScrollbarWithCurrentAngle()
    {
        float angle = transform.eulerAngles.y;
        float value = angle / 360.0f + 0.5f;
        while (value > 1.0f) value -= 1.0f;
        while(value <= 0.0f) value += 1.0f;
        //scrollbar.SetValueWithoutNotify(value);
    }

    private void Turn(float speed)
    {
        float angle = speed * Time.deltaTime;
        if (isTurningRight) angle *= -1;
        transform.Rotate(0, angle, 0);

        float value = transform.eulerAngles.y / 360.0f + 0.5f;
        while (value > 1.0f) value -= 1.0f;
        while (value < 0.0f) value += 1.0f;
        scrollbar.SetValueWithoutNotify(value);
        changeRotateEvent.Invoke(transform.eulerAngles.y);
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
        SetCamera();
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
        SetCamera();
    }

    public void SetAimOffset(float newAimOffset)
    {
        aimOffset = newAimOffset;
        SetCamera();
    }

    public void SetRotateSpeed(float newRotateSpeed)
    {
        rotateSpeed = newRotateSpeed;
    }

    public void SwitchTurnDirection()
    {
        isTurningRight = !isTurningRight;
    }

    private void OnDestroy()
    {
        if (scrollbar != null)
        {
            scrollbar.onValueChanged.RemoveAllListeners();
        }
    }
}