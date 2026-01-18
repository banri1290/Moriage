using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// チョビンたちの行動を管理するクラス
/// </summary>
public class ChobinManager : GameSystem
{
    [SerializeField] private Transform arrangeSpot;

    private ChobinBehaviour[] chobinBehaviours;
    private ChobinCooking[] chobinCookings;
    private Transform[][] kitchenLines;
    private float[] cookingTime;

    private int currentCookingChobinCount = 0;
    private int currentArrangedChobinIndex = 0;
    private int[] kitchenLineCount;
    private int[] chobinWaitingCount;

    private UnityEvent onArrange=new();
    
    public int ChobinIngredientIndex(int chobinIndex)
    {
        if (chobinIndex >= chobinCookings.Length)
        {
            Debug.LogError("チョビンの番号が不正です。");
            return 0;
        }
        return chobinCookings[chobinIndex].IngredientIndex;
    }
    public int ChobinMethodIndex(int chobinIndex, int methodIndex)
    {
        if (chobinIndex >= chobinCookings.Length)
        {
            Debug.LogError("チョビンの番号が不正です。");
            return 0;
        }
        else if (methodIndex >= chobinCookings[chobinIndex].CookingMethodIndex.Length)
        {
            Debug.LogWarning("調理法の番号が不正です。");
            methodIndex = chobinCookings[chobinIndex].CookingMethodIndex.Length - 1;
        }
        return chobinCookings[chobinIndex].CookingMethodIndex[methodIndex];
    }
    public int CurrentArrangedIngredientIndex => ChobinIngredientIndex(currentArrangedChobinIndex);
    public int[] CurrentArrangedMethodIndexes => chobinCookings[currentArrangedChobinIndex].CookingMethodIndex;

    private Transform KitchenSpot(int methodIndex, int lineIndex)
    {
        int index = lineIndex;
        if (lineIndex >= kitchenLines[methodIndex].Length)
        {
            index = kitchenLines[methodIndex].Length - 1;
        }
        return kitchenLines[methodIndex][index];
    }

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        if (arrangeSpot == null)
        {
            Debug.LogError("ChobinManager: Arrange spot is not assigned.");
            settingsAreCorrect = false;
        }

        return settingsAreCorrect;
    }

    public void Init(
        ChobinBehaviour[] _chobinBehaviours,
        ChobinCooking[] _chobinCookings,
        Transform[][] _kitchenLines,
        float[] _cookingTime,
        int methodLength
    )
    {
        currentCookingChobinCount = 0;
        kitchenLines = _kitchenLines;
        cookingTime = _cookingTime;
        if (kitchenLines.Length != cookingTime.Length)
        {
            Debug.LogError("キッチンの数と料理時間の数が一致しません。マズいですよ！");
            return;
        }
        kitchenLineCount = new int[kitchenLines.Length];
        for (int i = 0; i < kitchenLines.Length; i++)
        {
            kitchenLineCount[i] = 0;
        }
        chobinBehaviours = _chobinBehaviours;
        chobinCookings = _chobinCookings;
        chobinWaitingCount = new int[chobinCookings.Length];
        for (int i = 0; i < chobinCookings.Length; i++)
        {
            chobinWaitingCount[i] = 0;
            chobinCookings[i].Init(methodLength);
            chobinBehaviours[i].Init(i);
        }
    }

    public void SetOnArrangeCallback(UnityAction action)
    {
        onArrange.RemoveAllListeners();
        onArrange.AddListener(action);
    }

    public void SetChobinIngredient(int chobinIndex, int ingredientIndex)
    {
        chobinCookings[chobinIndex].SetIngredientIndex(ingredientIndex);
    }

    public void SetChobinMethod(int chobinIndex, int chobinMethodIndex, int methodIndex)
    {
        chobinCookings[chobinIndex].SetCookingMethodIndex(chobinMethodIndex, methodIndex);
    }

    /// <summary>
    /// チョビンが目的地に到達したときに呼び出されるコールバック
    /// </summary>
    /// <param name="chobinIndex">到達したチョビンのインデックス</param>
    private void OnChobinReachTarget(int chobinIndex)
    {
        ChobinCooking chobinCooking = chobinCookings[chobinIndex];
        switch (chobinCooking.ChobinStatus)
        {
            case ChobinCooking.Status.Waiting:
                break;
            case ChobinCooking.Status.Cooking: // 列の最前に来たら調理開始
                if (chobinWaitingCount[chobinIndex] == 0)
                    StartCookingMethod(chobinIndex, chobinCooking.CurrentCookingMethodIndex);
                break;
            case ChobinCooking.Status.GoingToArrange:
                chobinCooking.SetStatus(ChobinCooking.Status.endCooking);
                chobinBehaviours[chobinIndex].SetTarget(chobinBehaviours[chobinIndex].WaitingSpot);
                currentArrangedChobinIndex = chobinIndex;
                onArrange.Invoke();
                break;
            case ChobinCooking.Status.endCooking:
                chobinCooking.SetStatus(ChobinCooking.Status.Waiting);
                break;
        }
    }

    /// <summary>
    /// チョビンの調理が完了したときに呼び出されるコールバック
    /// </summary>
    /// <param name="chobinIndex">調理を完了したチョビンのインデックス</param>
    private void OnChobinCookingComplete(int chobinIndex)
    {
        ChobinCooking chobinCooking = chobinCookings[chobinIndex];
        ProceedWaitingLine(chobinCooking.CurrentCookingMethodIndex);
        chobinCooking.ProceedCooking();
        if (chobinCooking.ChobinStatus == ChobinCooking.Status.Cooking)
        {
            ChobinGoToNextKitchenLine(chobinIndex);
        }
        else // 盛り付けへ移行
        {
            currentCookingChobinCount--;
            chobinBehaviours[chobinIndex].SetTarget(arrangeSpot);
        }
    }

    /// <summary>
    /// 調理中のチョビンのターゲット指定
    /// </summary>
    /// <param name="waitingCount"></param>
    /// <param name="target"></param>
    private void SetKitchenSpot(int chobinIndex, int waitingCount, Transform target)
    {
        chobinWaitingCount[chobinIndex] = waitingCount;
        chobinBehaviours[chobinIndex].SetTarget(target);
    }

    /// <summary>
    /// チョビンに次の調理場へ向かわせるメソッド
    /// </summary>
    /// <param name="chobinIndex"></param>
    private void ChobinGoToNextKitchenLine(int chobinIndex)
    {
        int methodIndex = chobinCookings[chobinIndex].CurrentCookingMethodIndex;
        Transform target = KitchenSpot(methodIndex, kitchenLineCount[methodIndex]);
        SetKitchenSpot(chobinIndex, kitchenLineCount[methodIndex], target);
        kitchenLineCount[methodIndex]++;
    }

    /// <summary>
    /// チョビンに料理を開始させるメソッド
    /// </summary>
    /// <param name="chobinIndex"></param>
    private void StartCooking(int chobinIndex)
    {
        ChobinCooking chobinCooking = chobinCookings[chobinIndex];
        ChobinBehaviour chobinBehaviour = chobinBehaviours[chobinIndex];
        chobinBehaviour.SetEvents(OnChobinReachTarget, OnChobinCookingComplete);
        chobinCooking.StartCooking();
        ChobinGoToNextKitchenLine(chobinIndex);
        currentCookingChobinCount++;
    }

    /// <summary>
    /// 全てのチョビンに料理を開始させるメソッド
    /// </summary>
    public void StartAllCooking()
    {
        for (int i = 0; i < chobinCookings.Length; i++)
        {
            StartCooking(i);
        }
    }

    /// <summary>
    /// チョビンが調理を待つ列を進めるメソッド
    /// </summary>
    /// <param name="methodIndex">調理法の番号</param>
    /// <param name="waitingLine">調理を待つ列</param>
    public void ProceedWaitingLine(int methodIndex)
    {
        kitchenLineCount[methodIndex]--;
        for (int i = 0; i < chobinCookings.Length; i++)
        {
            if (chobinCookings[i].CurrentCookingMethodIndex == methodIndex)
            {
                int waitingCount = chobinWaitingCount[i] - 1;
                if (waitingCount < 0) continue;
                Transform target = KitchenSpot(methodIndex, waitingCount);
                SetKitchenSpot(i, waitingCount, target);
            }
        }
    }

    /// <summary>
    /// チョビンの調理作業開始
    /// </summary>
    /// <param name="chobinIndex"></param>
    /// <param name="methodIndex"></param>
    public void StartCookingMethod(int chobinIndex, int methodIndex)
    {
        float time = cookingTime[methodIndex];
        chobinBehaviours[chobinIndex].SetTimer(time);
    }
}
