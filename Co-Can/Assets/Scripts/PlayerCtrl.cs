using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;

    private float speed;
    private bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerRigidbody == null)
        {
            TryGetComponent<Rigidbody>(out playerRigidbody);
        }
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
        }
        else
        {
            speed -= accel * Time.deltaTime;
            if (speed < 0)
            {
                speed = 0;
            }
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = transform.forward * speed;
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward, Space.Self);
        }
    }

    public void SetMoving(bool _isMoving)
    {
        isMoving = _isMoving;
    }
}
