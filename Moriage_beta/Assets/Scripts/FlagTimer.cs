using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// フラグ付きのタイマー
/// </summary>
public class FlagTimer
{
    private class EventWithFloat : UnityEvent<float> { }

    private float duration = 2f;
    public bool flag { get; private set; }
    public float time { get; private set; }
    private UnityEvent onComplete;
    private EventWithFloat onUpdate;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_duration">タイマーの時間</param>
    public FlagTimer(float _duration)
    {
        duration = _duration;
        flag = false;
        time = 0f;
        onComplete = new UnityEvent();
        onUpdate = new EventWithFloat();
    }

    /// <summary>
    /// タイマー完了時のリスナーを設定
    /// </summary>
    /// <param name="action">完了時に実行するアクション</param>
    public void SetListenerOnComplete(UnityAction action)
    {
        onComplete.RemoveAllListeners();
        onComplete.AddListener(action);
    }

    /// <summary>
    /// タイマー更新時のリスナーを設定
    /// </summary>
    /// <param name="action">更新時に実行するアクション</param>
    public void SetListenerOnUpdate(UnityAction<float> action)
    {
        onUpdate.RemoveAllListeners();
        onUpdate.AddListener(action);
    }

    /// <summary>
    /// タイマーを開始
    /// </summary>
    public void Set()
    {
        flag = true;
        time = 0f;
    }

    /// <summary>
    /// タイマーを更新
    /// </summary>
    public void Update()
    {
        if (flag)
        {
            time += Time.deltaTime;
            onUpdate?.Invoke(time/duration);
            if (time >= duration)
            {
                flag = false;
                time = 0f;
                onComplete?.Invoke();
            }
        }
    }
}