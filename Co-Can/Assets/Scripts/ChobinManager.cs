using UnityEngine;

public class ChobinManager : GameSystem
{
    private int currentCookingNum = 0;

    public int CurrentCookingNum => currentCookingNum;

    public override bool CheckSettings()
    {
        bool AllSettingsAreCorrect = true;
        // ここに設定チェックのコードを追加

        return AllSettingsAreCorrect;
    }

    public void Init()
    {
        Debug.Log("ChobinCtrlの初期化が完了しました。");
    }

    public void IncrementCookingNum()
    {
        currentCookingNum++;
    }

    public void DecrementCookingNum()
    {
        currentCookingNum--;
    }
}
