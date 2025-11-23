using UnityEngine;
using TMPro;

public class SlotController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText; // 真ん中のテキスト
    [SerializeField] private string[] texts; // 切り替え候補
    private int currentIndex = 0;

    // 次のテキストへ
    public void NextItem()
    {
        currentIndex=(currentIndex + 1) % texts.Length; // 次のインデックスへ、最後は最初に戻る
        UpdateText();
    }

    // 前のテキストへ
    public void PreviousItem()
    {
        currentIndex= (currentIndex - 1 + texts.Length) % texts.Length; // 前のインデックスへ、最初は最後に戻る
        UpdateText();
    }

    // テキスト更新
    private void UpdateText()
    {
        displayText.text = texts[currentIndex];
    }

    // 最初に表示
    private void Start()
    {
        UpdateText();
    }
}