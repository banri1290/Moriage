using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// 食材を管理するクラス
/// </summary>
public class IngredientsManager : GameSystem
{
    [Serializable]
    class Ingredient
    {
        [SerializeField]private string name;
        [SerializeField]private Sprite icon;

        public string Name => name;
        public Sprite Icon => icon;
    }

    [SerializeField] private Ingredient[] ingredients;
    public int IngredientLength => ingredients.Length;
    public Sprite[] IngredientIcons=> ingredients.Select(ing => ing.Icon).ToArray();

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        if (ingredients == null || ingredients.Length == 0)
        {
            Debug.LogError("IngredientsManager: No ingredients defined.");
            settingsAreCorrect = false;
        }
        else
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (string.IsNullOrEmpty(ingredients[i].Name))
                {
                    Debug.LogError($"IngredientsManager: Ingredient at index {i} has no name.");
                    settingsAreCorrect = false;
                }
            }
        }

        return settingsAreCorrect;
    }
}
