using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ゲーム進行に関連するクラスの元となる継承元クラス
/// </summary>
public abstract class GameSystem : MonoBehaviour
{
    /// <summary>
    /// Inspectorで設定された値が正しいかチェックする抽象メソッド
    /// </summary>
    public abstract bool CheckSettings();
    private UnityEvent onValidateEvent = new UnityEvent();

    /// <summary>
    /// 自身のパラメータが変更されたとき、上位クラスがそれを検知できるようにする
    /// </summary>
    /// <param name="action">設定するアクション</param>
    public void SetListenerOnValidate(UnityAction action)
    {
        onValidateEvent.RemoveAllListeners();
        onValidateEvent.AddListener(action);
    }

    /// <summary>
    /// Inspectorの値が変更されたときに呼び出されるシステム関数
    /// </summary>
    void OnValidate()
    {
        onValidateEvent?.Invoke();
    }
}
