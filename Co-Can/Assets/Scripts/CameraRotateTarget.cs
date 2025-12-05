using Unity.VisualScripting;
using UnityEngine;

public class CameraRotateTarget : MonoBehaviour
{
    [SerializeField] private RoundCamera roundCamera;
    [SerializeField] private Transform target;
    [SerializeField] private float maxAngularSpeed = 10f;
    [SerializeField] private float deadzone = 0.001f;
    [SerializeField] private bool playerIsFront;
    private Transform cameraPivot;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null) target = transform;
        cameraPivot = roundCamera.transform;
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
            //target.position = cameraPivot.position + targetDirection.normalized * deadzone;
        }
        float cos = targetDirection.z / targetDistance;
        if (cos < -1) cos = -1; if (cos > 1) cos = 1;
        float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
        if (targetDirection.x < 0) angle *= -1;

        float nowAngle = roundCamera.transform.eulerAngles.y;
        float deltaAngle = angle - nowAngle;
        while (deltaAngle > 180) deltaAngle -= 360;
        while (deltaAngle < -180) deltaAngle += 360;
        if (Mathf.Abs(deltaAngle) > maxAngularSpeed * Time.deltaTime)
        {
            angle = nowAngle + Mathf.Sign(deltaAngle) * maxAngularSpeed * Time.deltaTime;
        }

        roundCamera.RotateCamera(angle);
    }
}
