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
    [SerializeField] private CommandUiSetting commandUiSetting;
    [SerializeField] private CommandUiManager commandUiManager;
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
        if (commandUiSetting != null && commandUiManager != null)
        {
            commandUiSetting.SetCommandUiManager(commandUiManager);
        }

        List<bool> settingsAreCorrect = new List<bool>
        {
            CheckSettingsOfGameSystem(ingredientsManager,"食材"),
            CheckSettingsOfGameSystem(cookingMethodManager,"調理法"),
            CheckSettingsOfGameSystem(chobinsSetting,"チョビン"),
            CheckSettingsOfGameSystem(chobinManager,"チョビンマネージャー"),
            CheckSettingsOfGameSystem(commandUiSetting,"指示UI"),
            CheckSettingsOfGameSystem(commandUiManager,"指示UIマネージャー"),
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
        InitCookingCommandUI();
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

    /// <summary>
    /// 指示UIの初期化
    /// </summary>
    private void InitCookingCommandUI()
    {
        commandUiSetting.Init(
            chobinsSetting.CookingMethodLength,
            chobinsSetting.ChobinIcons,
            ingredientsManager.IngredientIcons,
            cookingMethodManager.GetMethodIcons()
            );
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
        commandUiManager.SetButtonEvents(
            SelectPreviousChobin,
            SelectNextChobin,
            SelectPreviousIngredient,
            SelectNextIngredient,
            SelectPreviousMethod,
            SelectNextMethod
            );
        SetCommandUi(0);
    }

    /// <summary>
    /// 前のチョビンを選択する
    /// </summary>
    private void SelectPreviousChobin()
    {
        int nowChobinLength = commandUiManager.CurrentChobinIndex;
        int chobinIndex = (nowChobinLength - 1 + chobinsSetting.ChobinLength) % chobinsSetting.ChobinLength;
        SetCommandUi(chobinIndex);
    }

    /// <summary>
    /// 次のチョビンを選択する
    /// </summary>
    private void SelectNextChobin()
    {
        int nowChobinLength = commandUiManager.CurrentChobinIndex;
        int chobinIndex = (nowChobinLength + 1) % chobinsSetting.ChobinLength;
        SetCommandUi(chobinIndex);
    }

    private void SelectPreviousIngredient()
    {
        int chobinIndex = commandUiManager.CurrentChobinIndex;
        int nowIngredientIndex = chobinManager.ChobinIngredientIndex(chobinIndex);
        int ingredientIndex = (nowIngredientIndex - 1 + ingredientsManager.IngredientLength) % ingredientsManager.IngredientLength;
        chobinManager.SetChobinIngredient(chobinIndex, ingredientIndex);
        commandUiManager.SetIngredientUiIcon(ingredientIndex, chobinManager.ChobinIngredientIndex(chobinIndex));
    }

    private void SelectNextIngredient()
    {
        int chobinIndex = commandUiManager.CurrentChobinIndex;
        int nowIngredientIndex = chobinManager.ChobinIngredientIndex(chobinIndex);
        int ingredientIndex = (nowIngredientIndex + 1) % ingredientsManager.IngredientLength;
        chobinManager.SetChobinIngredient(chobinIndex, ingredientIndex);
        commandUiManager.SetIngredientUiIcon(ingredientIndex, chobinManager.ChobinIngredientIndex(chobinIndex));
    }

    /// <summary>
    /// チョビンの調理法を前のものに設定する
    /// </summary>
    /// <param name="chobinMethodIndex">調理工程のインデックス</param>
    private void SelectPreviousMethod(int chobinMethodIndex)
    {
        int chobinIndex = commandUiManager.CurrentChobinIndex;
        int nowMethodIndex = chobinManager.ChobinMethodIndex(chobinIndex, chobinMethodIndex);
        int methodIndex = (nowMethodIndex - 1 + cookingMethodManager.CookingMethodLength) % cookingMethodManager.CookingMethodLength;
        chobinManager.SetChobinMethod(chobinIndex, chobinMethodIndex, methodIndex);
        commandUiManager.SetMethodUiIcon(chobinMethodIndex, chobinManager.ChobinMethodIndex(chobinIndex, chobinMethodIndex));
    }

    /// <summary>
    /// チョビンの調理法を次のものに設定する
    /// </summary>
    /// <param name="chobinMethodIndex">調理工程のインデックス</param>
    private void SelectNextMethod(int chobinMethodIndex)
    {
        int chobinIndex = commandUiManager.CurrentChobinIndex;
        int nowMethodIndex = chobinManager.ChobinMethodIndex(chobinIndex, chobinMethodIndex);
        int methodIndex = (nowMethodIndex + 1) % cookingMethodManager.CookingMethodLength;
        chobinManager.SetChobinMethod(chobinIndex, chobinMethodIndex, methodIndex);
        commandUiManager.SetMethodUiIcon(chobinMethodIndex, chobinManager.ChobinMethodIndex(chobinIndex, chobinMethodIndex));
    }

    /// <summary>
    /// 現在選択されているチョビンの調理法をUIに反映する
    /// </summary>
    private void SetCommandUi(int chobinIndex)
    {
        commandUiManager.SetChobinIndex(chobinIndex);
        commandUiManager.SetIngredientUiIcon(
            chobinIndex, chobinManager.ChobinIngredientIndex(chobinIndex)
            );
        for (int i = 0; i < chobinsSetting.CookingMethodLength; i++)
        {
            int chobinMethodIndex = chobinManager.ChobinMethodIndex(chobinIndex, i);
            commandUiManager.SetMethodUiIcon(i, chobinMethodIndex);
        }
    }

    public void StartCooking()
    {
        commandUiManager.HideUi();
        dishMaker.StartMakingDish(chobinsSetting.CookingMethodLength);
        chobinManager.StartAllCooking();
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
