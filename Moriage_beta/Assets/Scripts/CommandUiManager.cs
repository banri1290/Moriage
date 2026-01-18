using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 指示UIを管理するクラス
/// </summary>
public class CommandUiManager : GameSystem
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private bool showUiInEditor = true;
    [SerializeField] private bool showUiOnStart = true;

    /// <summary>チョビン選択UIのアイコンImage</summary>
    private Image ChobinUiIcon;
    /// <summary>チョビン選択UIの「前へ」ボタン</summary>
    private Button ChobinUiPreviousButton;
    /// <summary>チョビン選択UIの「次へ」ボタン</summary>
    private Button ChobinUiNextButton;
    /// <summary>チョビン選択UIのアイコンとして使用するスプライトの配列</summary>
    private Sprite[] ChobinUiIconSprite;

    private Image IngredientUiIcon;
    private Button IngredientUiPreviousButton;
    private Button IngredientUiNextButton;
    private Sprite[] IngredientUiIconSprite;

    /// <summary>調理法選択UIのアイコンImageの配列</summary>
    private Image[] methodUiIcon;
    /// <summary>調理法選択UIの「前へ」ボタンの配列</summary>
    private Button[] methodUiPreviousButton;
    /// <summary>調理法選択UIの「次へ」ボタンの配列</summary>
    private Button[] methodUiNextButton;
    /// <summary>調理法選択UIのアイコンとして使用するスプライトの配列</summary>
    private Sprite[] methodUiIconSprite;

    /// <summary>現在選択されているチョビンのインデックス</summary>
    private int currentChobinIndex = 0;

    /// <summary>現在選択されているチョビンのインデックスを取得する</summary>
    public int CurrentChobinIndex => currentChobinIndex;

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;
        if (uiCanvas == null)
        {
            Debug.LogError("CommandUiManager: UI canvas is not assigned.");
            settingsAreCorrect = false;
        }
        else
        {
            uiCanvas.gameObject.SetActive(showUiInEditor);
        }

        return settingsAreCorrect;
    }

    void Start()
    {
#if UNITY_EDITOR
        uiCanvas.gameObject.SetActive(showUiOnStart);
#else
        HideUi();
#endif
    }

    public void SetUi(
        Image _chobinUiIcon,
        Sprite[] _chobinUiIconSprite,
        Button _chobinUiPreviousButton,
        Button _chobinUiNextButton,

        Image _ingredientUiIcon,
        Sprite[] _ingredientUiIconSprite,
        Button _ingredientUiPreviousButton,
        Button _ingredientUiNextButton,

        Image[] _methodUiIcon,
        Sprite[] _methodUiIconSprite,
        Button[] _methodUiPreviousButton,
        Button[] _methodUiNextButton
    )
    {
        ChobinUiIcon = _chobinUiIcon;
        ChobinUiIconSprite = _chobinUiIconSprite;
        ChobinUiPreviousButton = _chobinUiPreviousButton;
        ChobinUiNextButton = _chobinUiNextButton;

        IngredientUiIcon = _ingredientUiIcon;
        IngredientUiIconSprite = _ingredientUiIconSprite;
        IngredientUiPreviousButton = _ingredientUiPreviousButton;
        IngredientUiNextButton = _ingredientUiNextButton;

        methodUiIcon = _methodUiIcon;
        methodUiPreviousButton = _methodUiPreviousButton;
        methodUiNextButton = _methodUiNextButton;
        methodUiIconSprite = _methodUiIconSprite;
    }

    public void SetButtonEvents(
        UnityAction chobinPreviousAction,
        UnityAction chobinNextAction,
        UnityAction ingredientPreviousAction,
        UnityAction ingredientNextAction,
        UnityAction<int> methodPreviousAction,
        UnityAction<int> methodNextAction
        )
    {
        SetSelectButtonAction(ChobinUiPreviousButton, chobinPreviousAction, ChobinUiNextButton, chobinNextAction);
        SetSelectButtonAction(IngredientUiPreviousButton, ingredientPreviousAction, IngredientUiNextButton, ingredientNextAction);
        for (int i = 0; i < methodUiIcon.Length; i++)
        {
            int index = i;
            SetSelectButtonAction(
                methodUiPreviousButton[index], () => methodPreviousAction.Invoke(index),
                methodUiNextButton[index], () => methodNextAction.Invoke(index)
                );
        }
    }

    /// <summary>
    /// 選択中のチョビンのインデックスを設定し、UIに反映
    /// </summary>
    /// <param name="chobinIndex">設定するチョビンのインデックス</param>
    public void SetChobinIndex(int chobinIndex)
    {
        currentChobinIndex = chobinIndex;
        ChobinUiIcon.sprite = ChobinUiIconSprite[chobinIndex];
    }

    public void SetIngredientUiIcon(int imageIndex, int spriteIndex)
    {
        IngredientUiIcon.sprite = IngredientUiIconSprite[spriteIndex];
    }

    /// <summary>
    /// 調理法UIのアイコンを設定
    /// </summary>
    /// <param name="imageIndex">アイコンImageのインデックス</param>
    /// <param name="spriteIndex">スプライト配列のインデックス</param>
    public void SetMethodUiIcon(int imageIndex, int spriteIndex)
    {
        methodUiIcon[imageIndex].sprite = methodUiIconSprite[spriteIndex];
    }

    /// <summary>
    /// 左右の選択ボタンにアクションを設定
    /// </summary>
    private void SetSelectButtonAction(Button pButton, UnityAction pAction, Button nButton, UnityAction nAction)
    {
        SetListener(pButton.onClick, pAction);
        SetListener(nButton.onClick, nAction);
    }

    /// <summary>
    /// UnityEventにリスナーを設定（既存のリスナーは削除）
    /// </summary>
    private void SetListener(UnityEvent _event, UnityAction action)
    {
        _event.RemoveAllListeners();
        _event.AddListener(action);
    }

    public void ShowUi()
    {
        uiCanvas.gameObject.SetActive(true);
    }

    public void HideUi()
    {
        uiCanvas.gameObject.SetActive(false);
    }
}
