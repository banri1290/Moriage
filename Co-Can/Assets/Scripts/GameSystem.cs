using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class GameSystem : MonoBehaviour
{
    [NonSerialized]public UnityEvent CheckAllSettings = new();

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            CheckAllSettings?.Invoke();
        }
    }
#endif 

    /// <summary>
    /// このコンポーネントに必要な設定が正しく行われているかを検証します。
    /// </summary>
    public abstract bool CheckSettings();
}
