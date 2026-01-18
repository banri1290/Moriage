using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
[RequireComponent(typeof(NavMeshAgent))]
#endif

/// <summary>
/// チョビンの挙動を管理するクラス
/// </summary>
public class ChobinBehaviour : MonoBehaviour
{
    private class EventWithInt : UnityEvent<int> { }

    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float accel = 5f;
    [SerializeField] private int chobinIndex;
    [SerializeField] private Transform waitingSpot;

    [HideInInspector]
    [SerializeField] private NavMeshAgent agent;

    private bool isMoving;
    private FlagTimer timer;
    private Transform currentTarget;
    private EventWithInt onReachTargetEvent = new();
    private EventWithInt onTimerCompleteEvent = new();

    public Transform WaitingSpot => waitingSpot;

    /// <summary>
    /// 現在のターゲットまでの距離を計算
    /// </summary>
    /// <returns>ターゲットまでの距離</returns>
    private float DistanceToTarget()
    {
        if (currentTarget == null)
        {
            Debug.LogError("ChobinBehaviour: Current target is null.");
            return 0f;
        }
        else
        {
            Vector3 transformPosition = new(transform.position.x, 0, transform.position.z);
            Vector3 targetPosition = new(currentTarget.position.x, 0, currentTarget.position.z);
            return Vector3.Distance(transformPosition, targetPosition);
        }
    }

    /// <summary>
    /// 毎フレームの更新処理
    /// </summary>
    private void Update()
    {
        CheckReachedToTarget();
        timer?.Update();
    }

    /// <summary>
    /// intを引数に取るイベントにリスナーを設定
    /// </summary>
    /// <param name="_event">設定対象のイベント</param>
    /// <param name="action">設定するアクション</param>
    private void SetEventWithInt(EventWithInt _event, UnityAction<int> action)
    {
        _event.RemoveAllListeners();
        _event.AddListener(action);
    }

    /// <summary>
    /// onReachTargetEventとonTimerCompleteEventにリスナーを設定
    /// </summary>
    /// <param name="onReachTarget"></param>
    /// <param name="onTimerComplete"></param>
    public void SetEvents(UnityAction<int> onReachTarget, UnityAction<int> onTimerComplete)
    {
        SetEventWithInt(onReachTargetEvent, onReachTarget);
        SetEventWithInt(onTimerCompleteEvent, onTimerComplete);
    }

    public void SetChobinSettings(Transform _waitingSpot, float _maxSpeed, float _accel)
    {
        waitingSpot = _waitingSpot;
        maxSpeed = _maxSpeed;
        accel = _accel;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.speed = maxSpeed;
        agent.acceleration = accel;
    }

    /// <summary>
    /// チョビンの初期化処理
    /// </summary>
    /// <param name="_chobinIndex">チョビンのインデックス</param>
    public void Init(int _chobinIndex)
    {
        chobinIndex = _chobinIndex;
        onReachTargetEvent = new EventWithInt();
        onTimerCompleteEvent = new EventWithInt();
        isMoving = false;
    }

    /// <summary>
    /// 移動ターゲットを設定
    /// </summary>
    /// <param name="_target">ターゲットのTransform</param>
    /// <param name="action">ターゲット到達時に実行するアクション</param>
    public void SetTarget(Transform _target)
    {
        isMoving = true;
        currentTarget = _target;
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
            agent.SetDestination(_target.position);
#else
        agent.SetDestination(_target.position);
#endif
    }

    /// <summary>
    /// ターゲットに到達したかチェック
    /// </summary>
    private void CheckReachedToTarget()
    {
        if (currentTarget == null || !isMoving)
        {
            return;
        }
        if (DistanceToTarget() < 1e-3f)
        {
            isMoving = false;
            onReachTargetEvent?.Invoke(chobinIndex);
        }
    }

    /// <summary>
    /// タイマーを設定・開始
    /// </summary>
    /// <param name="time">タイマーの時間</param>
    public void SetTimer(float time)
    {
        timer = new FlagTimer(time);
        timer.SetListenerOnComplete(OnTimerComplete);
        timer.Set();
    }

    /// <summary>
    /// タイマー完了時に呼び出されるメソッド
    /// </summary>
    private void OnTimerComplete()
    {
        onTimerCompleteEvent?.Invoke(chobinIndex);
    }
}
