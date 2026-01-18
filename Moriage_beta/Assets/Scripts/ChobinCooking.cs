using UnityEngine;

/// <summary>
/// チョビンが料理を行うための状態管理クラス
/// </summary>
public class ChobinCooking : MonoBehaviour
{
    public enum Status
    {
        Waiting,
        Cooking,
        GoingToArrange,
        endCooking,
    }

    private int ingredientIndex = 0;
    private int[] cookingMethodIndex;

    private Status status = Status.Waiting;
    private int currentCookingCount = 0;

    public int IngredientIndex => ingredientIndex;
    public int[] CookingMethodIndex => cookingMethodIndex;
    public Status ChobinStatus => status;
    public int CurrentCookingCount => currentCookingCount;
    public int CurrentCookingMethodIndex => cookingMethodIndex[currentCookingCount];

    /// <summary>
    /// チョビンの料理関連パラメータを初期化
    /// </summary>
    /// <param name="cookingMethodLength">調理法の工程数</param>
    public void Init(int cookingMethodLength)
    {
        status = Status.Waiting;
        currentCookingCount = 0;

        ingredientIndex = 0;
        cookingMethodIndex = new int[cookingMethodLength];
        for (int i = 0; i < cookingMethodLength; i++)
        {
            cookingMethodIndex[i] = 0;
        }
    }

    /// <summary>
    /// 使用する食材のインデックスを設定
    /// </summary>
    /// <param name="_ingredientIndex">食材のインデックス</param>
    public void SetIngredientIndex(int _ingredientIndex)
    {
        ingredientIndex = _ingredientIndex;
    }

    /// <summary>
    /// 調理法のインデックスを設定
    /// </summary>
    /// <param name="index">工程のインデックス</param>
    /// <param name="number">調理法のインデックス</param>
    public void SetCookingMethodIndex(int index, int number)
    {
        cookingMethodIndex[index] = number;
    }

    /// <summary>
    /// 料理を開始
    /// </summary>
    public void StartCooking()
    {
        status = Status.Cooking;
        currentCookingCount = 0;
    }

    /// <summary>
    /// 調理工程を一つ進める
    /// </summary>
    public void ProceedCooking()
    {
        currentCookingCount++;
        if (currentCookingCount == cookingMethodIndex.Length)
        {
            currentCookingCount = 0;
            status = Status.GoingToArrange;
        }
    }

    /// <summary>
    /// チョビンのステータスを設定
    /// </summary>
    /// <param name="_status">設定するステータス</param>
    public void SetStatus(Status _status)
    {
        status = _status;
    }
}
