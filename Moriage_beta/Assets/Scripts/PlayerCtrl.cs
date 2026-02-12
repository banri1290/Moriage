using UnityEngine;
using UnityEngine.Events;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private UnityEvent onMoving;
    [SerializeField] private UnityEvent onStartedMoving;
    [SerializeField] private UnityEvent onStopped;

    private float speed;
    private bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = 0;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (isMoving)
        {
            speed += accel * Time.deltaTime;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            onMoving?.Invoke();
        }
        else
        {
            speed -= accel * Time.deltaTime;
            if (speed < 0)
            {
                speed = 0;
            }
        }
        transform.Translate(speed * Time.deltaTime * Vector3.forward,Space.Self);
    }
    
    public void SetMoving(bool _isMoving)
    {
        isMoving = _isMoving;
        if(isMoving)onStartedMoving?.Invoke();
        else onStopped?.Invoke();
    }
}
