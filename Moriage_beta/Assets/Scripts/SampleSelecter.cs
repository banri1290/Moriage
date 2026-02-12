using UnityEngine;

public class SampleSelecter : MonoBehaviour
{
    [SerializeField] private Selecter selecter;
    [SerializeField] private SelecterSetting setting;
    [SerializeField] private Sprite[] icons;

    private bool CheckSettings()
    {
        if (selecter == null)
        {
            Debug.LogError("Selecter is not assigned.");
            return false;
        }
        if (!selecter.CheckSettings()) return false;
        if (setting == null)
        {
            Debug.LogError("Setting is not assigned.");
            return false;
        }
        if (icons == null)
        {
            Debug.LogError("Icons are not assigned.");
            return false;
        }
        if (icons.Length == 0)
        {
            Debug.LogError("Icons are empty.");
            return false;
        }
        return true;
    }

    void Start()
    {
        if (!CheckSettings()) return;
        SetSettings();
        selecter.ResetSelection();
    }

    private void SetSettings()
    {
        setting.MatchIconSize(icons[0]);
        setting.MatchButtonSize();
        selecter.SetSettings(setting);
        selecter.SetIcons(icons);
        selecter.SetAction(
            (index)=>Debug.Log("Selected: " + index)
        );
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!CheckSettings()) return;
        SetSettings();
    }
#endif
}