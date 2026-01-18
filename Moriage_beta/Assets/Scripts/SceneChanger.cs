using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneChanger : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset targetScene;
#endif
    [SerializeField] private string sceneName;

    public void ChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetScene != null)
        {
            sceneName = targetScene.name;
        }
    }
#endif
}
