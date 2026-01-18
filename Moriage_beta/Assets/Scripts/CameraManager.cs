using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraRotater cameraRotater;
    [SerializeField] private CameraRotateTarget cameraRotateTarget;
    [SerializeField] private ControllerManager controllerManager;

    private void CheckAllSettings()
    {
        List<bool> settingsAreCorrect = new()
        {
            CheckSettingsOfGameSystem(cameraRotater,"カメラ回転システム"),
            CheckSettingsOfGameSystem(cameraRotateTarget,"カメラターゲット追従システム"),
            CheckSettingsOfGameSystem(controllerManager,"コントローラーシステム"),
        };

        if (settingsAreCorrect.Contains(false))
        {

        }
        else
        {
            Init();
        }
    }

    private bool CheckSettingsOfGameSystem(GameSystem system, string systemName)
    {
        bool result = true;
        if (system != null)
        {
            system.SetListenerOnValidate(CheckAllSettings);
            if (!system.CheckSettings())
            {
                Debug.LogError(systemName + "の設定に不備があります。");
                result = false;
            }
        }
        else
        {
            Debug.LogError(systemName + "が設定されていません。");
            result = false;
        }
        return result;
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        cameraRotater.Init();
        cameraRotateTarget.Init();
        controllerManager.Init();

        Debug.Log("CameraManager initialized.");
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        CheckAllSettings();
    }
#endif
}
