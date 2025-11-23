using UnityEngine;

public class CookingScoreCalclater : GameSystem
{
    [Header("スコア設定")]
    [Tooltip("味の好みによる基本スコア")]
    [SerializeField] private int tasteScore = 5;

    [Tooltip("待ち時間が短いほど加点するしきい値（秒）")]
    [SerializeField] private float waitingTimeThreshold = 15f;

    [Tooltip("調理ステップの重み")]
    [SerializeField] private int stepWeight = 3;

    public override bool CheckSettings() => true;

    public void Init()
    {
        Debug.Log("CookingScoreCalclaterの初期化が完了しました。");
    }

    // 🍳 Dish情報をもとにスコアを計算
 public int CalculateScore(Dish dish, GuestBehaviour guest)
{
    int score = 0;

    // 1️⃣ 部族の好み・嫌い
    foreach (var ingredient in dish.Ingredients)
    {
        if (guest.LikedIngredients.Contains(ingredient))
            score += 5;
        else if (guest.HatedIngredients.Contains(ingredient))
            score -= 5;
    }

    // 2️⃣ 提供時間
    if (dish.CookTime < 45f)
        score += 10;
    else if (dish.CookTime > 60f)
        score -= 3;

    // 3️⃣ 感情対応の食材
    bool hasEmotionIngredient = false;
    foreach (var ingredient in dish.Ingredients)
    {
        if (guest.EmotionIngredients.Contains(ingredient))
        {
            hasEmotionIngredient = true;
            break;
        }
    }
    score += hasEmotionIngredient ? 5 : -5;

    // 4️⃣ 調理工程
    switch (dish.Steps)
    {
        case 3: score += 10; break;
        case 2: score += 5; break;
        case 1: score += 0; break;
        case 0: score -= 10; break;
    }

    // スコア下限
    score = Mathf.Max(0, score);

    Debug.Log($"【スコア計算】材料:{dish.Ingredients.Count}個 工程:{dish.Steps} 調理時間:{dish.CookTime:F2}秒 → スコア:{score}");

    return score;
}
}