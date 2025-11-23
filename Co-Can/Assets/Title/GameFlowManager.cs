using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private Button returnButton;

    private int servedCount = 0; // 提供した人数カウント

    void Start()
    {
        // 最初は非表示
        if (returnButton != null)
            returnButton.gameObject.SetActive(false);

        // ボタンクリック時にタイトルに戻る処理を設定
        if (returnButton != null)
            returnButton.onClick.AddListener(ReturnToTitle);
    }

    /// <summary>
    /// 料理を提供したときに呼ぶ
    /// </summary>
    public void OnDishServed()
    {
        servedCount++;
        Debug.Log($"🍽️ 提供人数: {servedCount}");

        if (servedCount >= 5)
        {
            // 5人目でボタンを出す
            if (returnButton != null)
                returnButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// タイトルシーンに戻る処理
    /// </summary>
    private void ReturnToTitle()
    {
        SceneManager.LoadScene("TitleScene"); // ← タイトルシーン名に合わせて
    }
}