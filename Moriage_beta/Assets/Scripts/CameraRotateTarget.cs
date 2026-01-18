using UnityEngine;

public class CameraRotateTarget : GameSystem
{
    [SerializeField] private CameraRotater cameraRotater;
    [SerializeField] private Transform target;
    [SerializeField] private float maxAngularSpeed = 10f;
    [SerializeField] private float deadzone = 0.001f;
    [SerializeField] private bool playerIsFront;
    private Transform cameraPivot;

    public override bool CheckSettings()
    {
        bool result = true;

        if (cameraRotater == null)
        {
            Debug.LogError("CameraRotater is null");
            result = false;
        }

        if (target == null) target = transform;
        if (target == null)
        {
            Debug.LogError("Target is null");
            result = false;
        }

        return result;
    }

    public void Init()
    {
        cameraPivot = cameraRotater.Pivot;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        Vector3 targetDirection = target.position - cameraPivot.position;
        if (playerIsFront) targetDirection *= -1;
        float targetDistance = targetDirection.magnitude;
        if (targetDistance < deadzone)
        {
            return;
        }
        float cos = targetDirection.z / targetDistance;
        if (cos < -1) cos = -1; if (cos > 1) cos = 1;
        float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
        if (targetDirection.x < 0) angle *= -1;

        float nowAngle = cameraPivot.eulerAngles.y;
        float deltaAngle = angle - nowAngle;
        while (deltaAngle > 180) deltaAngle -= 360;
        while (deltaAngle < -180) deltaAngle += 360;
        if (Mathf.Abs(deltaAngle) > maxAngularSpeed * Time.deltaTime)
        {
            angle = nowAngle + Mathf.Sign(deltaAngle) * maxAngularSpeed * Time.deltaTime;
        }
        else
        {
            angle = nowAngle + deltaAngle;
        }

        cameraRotater.SetAngle(angle);
    }
}
