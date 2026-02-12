using System;
using UnityEngine;
using UnityEngine.Events;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float time = 1.0f;
    [SerializeField] private bool playOnAwake = false;

    private UnityEvent onAppear = new();

    private FlagTimer timer;
    public float TimeCount => timer.time;

    // Start is called before the first frame update
    void Start()
    {
        if (playOnAwake) Init();
        onAppear.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Set(float _time)
    {
        time = _time;
        playOnAwake = true;
        Init();
    }

    void Init()
    {
        timer = new FlagTimer(this, time);
        timer.SetListenerOnComplete(() => Destroy(gameObject));
        timer.Set();
    }

    public void SetAwake(float _time)
    {
        onAppear.AddListener(() => Set(_time));
    }
}
