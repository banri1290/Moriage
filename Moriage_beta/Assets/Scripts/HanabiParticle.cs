using UnityEngine.UI;
using UnityEngine;

public class HanabiParticle : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private Vector2 firstVelocity;
    [SerializeField] private Vector2 lastVelocity;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float firstSize;
    [SerializeField] private float lastSize;
    [SerializeField] private float fadeoutTime;
    [SerializeField] private bool playSelf = false;

    private bool isActive;
    private DestroyTimer timer;
    private Image myImage;
    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        if (playSelf)
        {
            Init(time);
            velocity = firstVelocity;
            isActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Action();
        }
    }

    void Action()
    {
        float timeRate = timer.TimeCount / time;

        velocity = firstVelocity * (1 - timeRate) + lastVelocity * timeRate;
        transform.localPosition += (Vector3)velocity * Time.deltaTime;
        transform.Rotate(Vector3.forward, angularSpeed * Time.deltaTime);
        transform.localScale = Vector3.one * (timeRate * (lastSize - firstSize) + firstSize);
        if (fadeoutTime > 0.0f && time - timer.TimeCount < fadeoutTime)
        {
            float r = myImage.color.r;
            float g = myImage.color.g;
            float b = myImage.color.b;
            float a = (time - timer.TimeCount) / fadeoutTime;
            myImage.color = new Color(r, g, b, a);
        }
    }

    public void Init(float _time)
    {
        playSelf = false;
        isActive = true;
        time = _time;
        if (TryGetComponent<DestroyTimer>(out DestroyTimer _timer))
        {
            timer = _timer;
        }
        else
        {
            timer = gameObject.AddComponent<DestroyTimer>();
        }
        timer.SetAwake(time);

        myImage = GetComponent<Image>();
    }

    public void SetMovement(Vector2 _firstVelocity, Vector2 _lastVelocity, float _angularSpeed)
    {
        firstVelocity = _firstVelocity;
        lastVelocity = _lastVelocity;
        velocity = firstVelocity;
        angularSpeed = _angularSpeed;
    }

    public void SetSize(float _firstSize, float _lastSize)
    {
        firstSize = _firstSize;
        lastSize = _lastSize;
    }

    public void SetFadeout(float _fadeoutTime)
    {
        fadeoutTime = _fadeoutTime;
    }

    public void SetFadeout()
    {
        fadeoutTime = time;
    }

    public void SetColor(Color _color)
    {
        if (myImage != null)
        {
            myImage.color = _color;
        }
    }
}