using UnityEngine;

public class CommandUiSetting : GameSystem
{
    [SerializeField] private Selecter selecterPrefab;
    [SerializeField] private RectTransform selecterParent;
    [SerializeField] private SelecterSetting selecterSetting;
    [SerializeField] private Vector2 selecterPositionBase;
    [SerializeField] private Vector2 selecterPositionGap;

    public override bool CheckSettings()
    {
        if (selecterPrefab == null)
        {
            Debug.LogError("Selecter is not assigned.");
            return false;
        }
        if (!selecterPrefab.CheckSettings())
        {
            return false;
        }
        return true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetChobinCount(int chobinCount)
    {
        
    }
}
