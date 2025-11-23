using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnToTitleButton : MonoBehaviour
{
    [SerializeField] private Button returnButton;
    [SerializeField] private string titleSceneName = "TitleScene"; // ← タイトルシーン名

    private void Start()
    {
        if (returnButton == null)
            returnButton = GetComponent<Button>();

        returnButton.onClick.AddListener(ReturnToTitle);
    }

    private void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }
}