using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// ゲーム進行全体を管理するコアスクリプト
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("設定確認用フラグ")]
    [SerializeField] private bool checkSettingsFlag = false;
    [Header("各種システム")]
    [SerializeField] private IngredientsManager ingredientsManager;
    [SerializeField] private CookingMethodManager cookingMethodManager;
    [SerializeField] private ChobinsSetting chobinsSetting;
    [SerializeField] private ChobinManager chobinManager;
    [SerializeField] private DishMaker dishMaker;

    /// <summary>
    /// ゲーム開始時の初期化処理
    /// </summary>
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 全てのGameSystemの設定をチェック
    /// </summary>
    private void CheckAllSettings()
    {
        if(checkSettingsFlag) checkSettingsFlag = false;

        if (chobinsSetting != null && chobinManager != null)
        {
            chobinsSetting.SetChobinManager(chobinManager);
        }

        List<bool> settingsAreCorrect = new List<bool>
        {
            CheckSettingsOfGameSystem(ingredientsManager,"食材"),
            CheckSettingsOfGameSystem(cookingMethodManager,"調理法"),
            CheckSettingsOfGameSystem(chobinsSetting,"チョビン"),
            CheckSettingsOfGameSystem(chobinManager,"チョビンマネージャー"),
            CheckSettingsOfGameSystem(dishMaker,"料理作成システム"),
        };

        if (settingsAreCorrect.Contains(false))
        {

        }
        else
        {
            Init();
        }
    }

    /// <summary>
    /// 指定されたGameSystemの設定をチェック
    /// </summary>
    /// <param name="gameSystem">チェック対象のGameSystem</param>
    /// <returns>設定が正しければtrue</returns>
    private bool CheckSettingsOfGameSystem(GameSystem gameSystem, string summary)
    {
        if (gameSystem == null)
        {
            Debug.LogError(summary + "が設定されていません。");
            return false;
        }
        gameSystem.SetListenerOnValidate(CheckAllSettings);
        if (!gameSystem.CheckSettings())
        {
            Debug.LogError(summary + "の設定に問題があります。");
            return false;
        }
        return true;
    }

    /// <summary>
    /// ゲームの初期化処理
    /// </summary>
    private void Init()
    {
        InitCookingMethod();
        InitChobins();
        Debug.Log("All settings are correct.");
    }

    /// <summary>
    /// 調理法クラスの初期化
    /// </summary>
    private void InitCookingMethod()
    {
        cookingMethodManager.Init();
    }

    /// <summary>
    /// チョビンの初期化
    /// </summary>
    private void InitChobins()
    {
        chobinsSetting.Init(
            cookingMethodManager.GetKitchenLines(),
            cookingMethodManager.GetCookingTime()
            );
        chobinManager.SetOnArrangeCallback(AddIngredientToDish);
    }

    private void AddIngredientToDish()
    {
        int ingredientId = chobinManager.CurrentArrangedIngredientIndex;
        int[] cookingMethods = chobinManager.CurrentArrangedMethodIndexes;
        dishMaker.AddIngredientToDish(ingredientId, cookingMethods);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Inspectorの値が変更されたときに呼び出されるシステム関数
    /// </summary>
    private void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            CheckAllSettings();
        }
    }
#endif
}
