using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        // ゲーム本編のシーンに切り替え
        SceneManager.LoadScene("CopyScene20251024");
    }
}