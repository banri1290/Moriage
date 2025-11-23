using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Material
{
    [Tooltip("材料の名前")]
    [SerializeField] private string name; // 材料の名前
    [Tooltip("材料の画像")]
    [SerializeField] private Sprite sprite; // 材料の画像

    public string Name => name; // 材料の名前を取得
    public Sprite Sprite => sprite; // 材料の画像を取得
}
[System.Serializable]
public struct Action
{
    [Tooltip("調理方法の名前（例：切る、焼く）")]
    [SerializeField] private string name;
    [Tooltip("調理法の画像")]
    [SerializeField] private Sprite sprite;
    [Tooltip("この調理を行う場所のTransform")]
    [SerializeField] private Transform kitchinSpot;

    public string Name => name; // 調理方法の名前を取得
    public Sprite Sprite => sprite; // 調理法の画像を取得
    public Transform KitchinSpot => kitchinSpot; //調理する場所を取得
}

public class Dish
{
    public List<string> Ingredients { get; private set; }
    public int Steps { get; set; }
    public float CookTime { get; set; }

    public Dish()
    {
        Ingredients = new List<string>();
        Steps = 0;
        CookTime = 0f;
    }

    public void AddIngredient(string ingredient)
    {
        if (!Ingredients.Contains(ingredient))
            Ingredients.Add(ingredient);
    }

    // ゲスト情報を渡す形に拡張可能
    public int CalculateScore(GuestBehaviour guest)
    {
        int score = 0;

        // 例: 材料数
        score += Ingredients.Count * 5;

        // 例: 調理工程
        score += Steps switch
        {
            3 => 10,
            2 => 5,
            1 => 0,
            _ => -10
        };

        // 例: 提供時間
        score += CookTime < 45f ? 10 : CookTime > 45f ? -3 : 0;

        // 例: 好み・嫌い・感情判定
        if (guest.LikedIngredients.Exists(i => Ingredients.Contains(i))) score += 5;
        if (guest.HatedIngredients.Exists(i => Ingredients.Contains(i))) score -= 5;
        if (guest.EmotionIngredients.Exists(i => Ingredients.Contains(i))) score += 5;
        else score -= 5;

        return score;
    }
}