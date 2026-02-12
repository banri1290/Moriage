using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// フラグ付きのタイマー
/// </summary>
public class FlagTimer
{
    private MonoBehaviour parent;
    private float duration = 2f;
    public bool flag { get; private set; }
    public float time { get; private set; }
    private UnityAction onComplete;
    private UnityAction<float> onUpdate;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_duration">タイマーの時間</param>
    public FlagTimer(MonoBehaviour _parent, float _duration)
    {
        parent = _parent;
        duration = _duration;
        flag = false;
        time = 0f;
        onComplete = null;
        onUpdate = null;
    }

    /// <summary>
    /// タイマー完了時のリスナーを設定
    /// </summary>
    /// <param name="action">完了時に実行するアクション</param>
    public void SetListenerOnComplete(UnityAction action)
    {
        onComplete = action;
    }

    /// <summary>
    /// タイマー更新時のリスナーを設定
    /// </summary>
    /// <param name="action">更新時に実行するアクション</param>
    public void SetListenerOnUpdate(UnityAction<float> action)
    {
        onUpdate = action;
    }

    /// <summary>
    /// タイマーを開始
    /// </summary>
    public void Set()
    {
        parent.StartCoroutine(Update());
    }

    /// <summary>
    /// タイマーを更新
    /// </summary>
    private IEnumerator Update()
    {
        flag = true;
        time = 0f;
        while (flag && time < duration)
        {
            float raito = time / duration;
            onUpdate?.Invoke(raito);
            yield return null;
            time += Time.deltaTime;
        }
        onUpdate?.Invoke(1f);
        flag = false;
        onComplete?.Invoke();
    }
}