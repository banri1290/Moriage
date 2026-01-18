using UnityEngine;

public class CameraRotater : GameSystem
{
    [SerializeField] Transform pivot;
    [SerializeField] Camera targetCamera;
    [SerializeField] float radius;
    [SerializeField] float height;
    [SerializeField] float offset;
    [SerializeField] float defaultAngle;

    [HideInInspector]
    [SerializeField] private Transform cameraTransform;

    public Transform Pivot => pivot;

    public override bool CheckSettings()
    {
        bool result = true;

        if (pivot == null)
        {
            Debug.LogError("Pivot is null");
            result = false;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            Debug.LogError("Target camera is null");
            result = false;
        }
        else
        {
            if (pivot != null)
            {
                if (!targetCamera.transform.IsChildOf(pivot))
                {
                    targetCamera.transform.parent = pivot;
                }
            }

            cameraTransform = targetCamera.transform;
        }

        return result;
    }

    public void Init()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        InitInEditor();
#endif
    }

    private void InitInEditor()
    {
        Vector3 pos = new(0, height, radius);
        cameraTransform.localPosition = pos;
        SetAngle(0);
        cameraTransform.forward = -pos + Vector3.forward * offset;
        SetAngle(defaultAngle);
    }

    public void SetAngle(float angle)
    {
        while (angle >= 360) angle -= 360;
        while (angle < 0) angle += 360;
        pivot.eulerAngles = new(0, angle, 0);
    }
}
