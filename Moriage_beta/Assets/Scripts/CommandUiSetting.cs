using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 指示UIの見た目に関する設定や動的な生成を行うクラス
/// </summary>
public class CommandUiSetting : GameSystem
{
    /// <summary>
    /// 画像のアスペクト比を維持したままリサイズする際のオプション
    /// </summary>
    public enum SizeOption
    {
        /// <summary>サイズ調整を行わない</summary>
        None,
        /// <summary>幅に合わせて、アスペクト比を維持したまま高さを調整する</summary>
        MatchAspectRatioBasedOnWidth,
        /// <summary>高さに合わせて、アスペクト比を維持したまま幅を調整する</summary>
        MatchAspectratioBasedOnHeight
    }

    /// <summary>
    /// ボタンUIに関する設定をまとめたクラス
    /// </summary>
    [System.Serializable]
    class ButtonSetting
    {
        [Tooltip("設定対象のボタンプレハブ")]
        public Button button;
        public Sprite sprite;
        [Tooltip("ボタンのサイズ")]
        public Vector2 buttonSize;
        [Tooltip("ボタンのリサイズオプション")]
        public SizeOption sizeOption;

        /// <summary>
        /// ボタン設定が正しいかチェック
        /// </summary>
        public bool CheckSettings()
        {
            bool settingsAreCorrect = true;
            if (button == null)
            {
                Debug.LogError("CookingCommandSetting: Button is not assigned.");
                settingsAreCorrect = false;
            }
            else if (!button.GetComponent<RectTransform>())
            {
                Debug.LogError("CookingCommandSetting: Button does not have a RectTransform component.");
                settingsAreCorrect = false;
            }
            return settingsAreCorrect;
        }
    }

    /// <summary>
    /// 選択UI（アイコンと左右のボタン）に関する設定をまとめたクラス
    /// </summary>
    [System.Serializable]
    class SelectorSetting
    {
        [Tooltip("UIを生成する親オブジェクト")]
        public Transform uiParent;
        [Tooltip("生成するUIオブジェクトの基名")]
        public string name;
        [Tooltip("UIの基準位置")]
        public Vector2 position;
        [Tooltip("UIを複数生成する際の位置の差分")]
        public Vector2 delta;
        [Tooltip("中央のアイコンから左右の選択ボタンまでの距離")]
        public Vector2 selectButtonDelta;
        [Tooltip("中央のアイコンのサイズ")]
        public Vector2 iconSize;
        [Tooltip("アイコンのサンプルスプライト（サイズ計算用）")]
        public Sprite iconSpriteSample;
        [Tooltip("アイコンのリサイズオプション")]
        public SizeOption iconSizeOption;
        [Tooltip("エディタ上でUIを強制的に再生成するかどうかのフラグ")]
        public bool resetUi;

        /// <summary>
        /// セレクター設定が正しいかチェック
        /// </summary>
        public bool CheckSettings(string instanceName)
        {
            bool settingsAreCorrect = true;
            if (uiParent == null)
            {
                Debug.LogError(instanceName + ": UI parent is not assigned");
                settingsAreCorrect = false;
            }
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError(instanceName + ": UI name is not assigned.");
                settingsAreCorrect = false;
            }

            return settingsAreCorrect;
        }
    }

    /// <summary>
    /// 動的に生成された選択UIのコンポーネント参照を保持するクラス
    /// </summary>
    [System.Serializable]
    class Selector
    {
        /// <summary>中央のアイコン</summary>
        public Image icon;
        /// <summary>前の項目を選択するボタン</summary>
        public Button previousButton;
        /// <summary>次の項目を選択するボタン</summary>
        public Button nextButton;
        /// <summary>アイコンのRectTransform</summary>
        public RectTransform iconRect;
        /// <summary>前へボタンのRectTransform</summary>
        public RectTransform previousButtonRect;
        /// <summary>次へボタンのRectTransform</summary>
        public RectTransform nextButtonRect;

        /// <summary>
        /// UI要素の位置を設定
        /// </summary>
        /// <param name="_position">設定する位置</param>
        public void SetPosition(Vector2 _position)
        {
            iconRect.anchoredPosition = _position;
        }
    }

    [Header("ボタンの共通設定")]
    [Tooltip("『前へ』ボタンの設定")]
    [SerializeField] private ButtonSetting previousButtonSetting;
    [Tooltip("『次へ』ボタンの設定")]
    [SerializeField] private ButtonSetting nextButtonSetting;

    [Header("UIごとの設定")]
    [Tooltip("チョビン選択UIの設定")]
    [SerializeField] private SelectorSetting chobinUiSetting;
    [Tooltip("食材選択UIの設定")]
    [SerializeField] private SelectorSetting ingredientUiSetting;
    [Tooltip("調理法選択UIの設定")]
    [SerializeField] private SelectorSetting methodUiSetting;

    [SerializeField] private CommandUiManager commandUiManager;

    /// <summary>生成されたチョビン選択UIの参照</summary>
    [HideInInspector]
    [SerializeField] private Selector chobinUiSelector;
    /// <summary>生成された食材選択UIの参照</summary>
    [HideInInspector]
    [SerializeField] private Selector ingredientUiSelector;
    /// <summary>生成された調理法選択UIの参照配列</summary>
    [HideInInspector]
    [SerializeField] private Selector[] methodUiSelector;

    /// <summary>調理法の工程数</summary>
    private int methodUiCount;

    /// <summary>チョビン選択UIのアイコン</summary>
    public Image ChobinUiIcon => chobinUiSelector.icon;
    /// <summary>チョビン選択UIの「前へ」ボタン</summary>
    public Button ChobinUiPreviousButton => chobinUiSelector.previousButton;
    /// <summary>チョビン選択UIの「次へ」ボタン</summary>
    public Button ChobinUiNextButton => chobinUiSelector.nextButton;

    /// <summary>食材選択UIのアイコン</summary>
    public Image IngredientUiIcon => ingredientUiSelector.icon;
    /// <summary>食材選択UIの「前へ」ボタン</summary>
    public Button IngredientUiPreviousButton => ingredientUiSelector.previousButton;
    /// <summary>食材選択UIの「次へ」ボタン</summary>
    public Button IngredientUiNextButton => ingredientUiSelector.nextButton;

    public Image[] MethodUiIcon
    => methodUiSelector.Select(x => x.icon).ToArray();
    public Button[] MethodUiPreviousButton
    => methodUiSelector.Select(x => x.previousButton).ToArray();
    public Button[] MethodUiNextButton
    => methodUiSelector.Select(x => x.nextButton).ToArray();

    public void SetCommandUiManager(CommandUiManager _commandUiManager)
    {
        commandUiManager = _commandUiManager;
    }

    /// <summary>
    /// Inspectorで設定された値が正しいかチェック
    /// </summary>
    public override bool CheckSettings()
    {
        bool settingsAreCorrect = true;

        if (!previousButtonSetting.CheckSettings())
        {
            settingsAreCorrect = false;
        }
        if (!nextButtonSetting.CheckSettings())
        {
            settingsAreCorrect = false;
        }
        if (!chobinUiSetting.CheckSettings("chobinUiSetting"))
        {
            settingsAreCorrect = false;
        }
        if (!ingredientUiSetting.CheckSettings("ingredientUiSetting"))
        {
            settingsAreCorrect = false;
        }
        if (!methodUiSetting.CheckSettings("methodUiSetting"))
        {
            settingsAreCorrect = false;
        }

        return settingsAreCorrect;
    }

    /// <summary>
    /// 指示UIの初期化
    /// </summary>
    /// <param name="_methodUiCount">調理法の工程数</param>
    public void Init(int _methodUiCount, Sprite[] chobinIcons,Sprite[] ingredientIcons,Sprite[] methodIcons)
    {
        SetSpriteSize(previousButtonSetting.sprite, ref previousButtonSetting.buttonSize, previousButtonSetting.sizeOption);
        SetSpriteSize(nextButtonSetting.sprite, ref nextButtonSetting.buttonSize, nextButtonSetting.sizeOption);
        SetSpriteSize(methodUiSetting.iconSpriteSample, ref methodUiSetting.iconSize, methodUiSetting.iconSizeOption);
        SetSpriteSize(ingredientUiSetting.iconSpriteSample, ref ingredientUiSetting.iconSize, ingredientUiSetting.iconSizeOption);
        SetSpriteSize(chobinUiSetting.iconSpriteSample, ref chobinUiSetting.iconSize, chobinUiSetting.iconSizeOption);

        methodUiCount = _methodUiCount;

#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (methodUiSetting.uiParent.childCount != methodUiCount || methodUiSetting.resetUi)
            {
                EditorApplication.delayCall += SetMethodUiObject;
            }
            else
            {
                SetMethodUiComponent();
            }

            if (ingredientUiSetting.uiParent.childCount != 1 || ingredientUiSetting.resetUi)
            {
                EditorApplication.delayCall += SetIngredientUiObject;
            }
            else
            {
                SetIngredientUiComponent();
            }

            if (chobinUiSetting.uiParent.childCount != 1 || chobinUiSetting.resetUi)
            {
                EditorApplication.delayCall += SetChobinUiObject;
            }
            else
            {
                SetChobinUiComponent();
            }
            return;
        }
#endif
        commandUiManager.SetUi(
            ChobinUiIcon,
            chobinIcons,
            ChobinUiPreviousButton,
            ChobinUiNextButton,
            IngredientUiIcon,
            ingredientIcons,
            IngredientUiPreviousButton,
            IngredientUiNextButton,
            MethodUiIcon,
            methodIcons,
            MethodUiPreviousButton,
            MethodUiNextButton
        );
    }

    /// <summary>
    /// チョビン選択UIのオブジェクトを生成・設定
    /// </summary>
    private void SetChobinUiObject()
    {
        ClearChildren(chobinUiSetting.uiParent);
        GameObject chobinUi = MakeSelector
        (
            chobinUiSetting.name,
            chobinUiSetting.uiParent,
            ref chobinUiSelector
            );
        SetChobinUiComponent();
        chobinUiSetting.resetUi = false;
#if UNITY_EDITOR
        EditorApplication.delayCall -= SetChobinUiObject;
#endif
    }

    /// <summary>
    /// チョビン選択UIのコンポーネントの形状を設定
    /// </summary>
    private void SetChobinUiComponent()
    {
        SetSelectorShape(chobinUiSelector, chobinUiSetting);
    }

    /// <summary>
    /// 食材選択UIのオブジェクトを生成・設定
    /// </summary>
    private void SetIngredientUiObject()
    {
        ClearChildren(ingredientUiSetting.uiParent);
        GameObject ingredientUi = MakeSelector
        (
            ingredientUiSetting.name,
            ingredientUiSetting.uiParent,
            ref ingredientUiSelector
            );
        SetIngredientUiComponent();
        ingredientUiSetting.resetUi = false;
#if UNITY_EDITOR
        EditorApplication.delayCall -= SetIngredientUiObject;
#endif
    }

    /// <summary>
    /// 食材選択UIのコンポーネントの形状を設定
    /// </summary>
    private void SetIngredientUiComponent()
    {
        SetSelectorShape(ingredientUiSelector, ingredientUiSetting);
    }

    /// <summary>
    /// 調理法選択UIのオブジェクトを生成・設定
    /// </summary>
    private void SetMethodUiObject()
    {
        methodUiSelector = new Selector[methodUiCount];

        ClearChildren(methodUiSetting.uiParent);
        for (int i = 0; i < methodUiCount; i++)
        {
            GameObject methodUi = MakeSelector
            (
                methodUiSetting.name + "_" + i,
                methodUiSetting.uiParent,
               ref methodUiSelector[i]
                );
        }
        SetMethodUiComponent();
        methodUiSetting.resetUi = false;
#if UNITY_EDITOR
        EditorApplication.delayCall -= SetMethodUiObject;
#endif
    }

    /// <summary>
    /// 調理法選択UIのコンポーネントの形状を設定
    /// </summary>
    private void SetMethodUiComponent()
    {
        for (int i = 0; i < methodUiCount; i++)
        {
            SetSelectorShape(methodUiSelector[i], methodUiSetting, i);
        }
    }

    /// <summary>
    /// 指定した調理法UIのアイコンを設定
    /// </summary>
    /// <param name="methodIndex">調理法UIのインデックス</param>
    /// <param name="icon">設定するスプライト</param>
    public void SetMethodUiIcon(int methodIndex, Sprite icon)
    {
        methodUiSelector[methodIndex].icon.sprite = icon;
    }

    /// <summary>
    /// 選択UIのオブジェクトを生成
    /// </summary>
    /// <param name="_name">オブジェクト名</param>
    /// <param name="parent">親のTransform</param>
    /// <param name="selector">生成したUI要素を格納するSelectorクラス</param>
    private GameObject MakeSelector(string _name, Transform parent, ref Selector selector)
    {
        GameObject baseObj = new()
        {
            name = _name
        };
        selector = new()
        {
            iconRect = baseObj.AddComponent<RectTransform>(),
            icon = baseObj.AddComponent<Image>(),
        };

        if (baseObj.GetComponent<CanvasRenderer>() == null)
            baseObj.AddComponent<CanvasRenderer>();
        baseObj.transform.SetParent(parent);

        GameObject pButtonObj = PrefabUtility.InstantiatePrefab(previousButtonSetting.button.gameObject) as GameObject;
        pButtonObj.transform.SetParent(baseObj.transform);
        selector.previousButton = pButtonObj.GetComponent<Button>();
        selector.previousButtonRect = selector.previousButton.GetComponent<RectTransform>();

        GameObject nButtonObj = PrefabUtility.InstantiatePrefab(nextButtonSetting.button.gameObject) as GameObject;
        nButtonObj.transform.SetParent(baseObj.transform);
        selector.nextButton = nButtonObj.GetComponent<Button>();
        selector.nextButtonRect = selector.nextButton.GetComponent<RectTransform>();

        return baseObj;
    }

    /// <summary>
    /// 選択UIの形状を設定
    /// </summary>
    /// <param name="selector">形状を設定するSelectorクラス</param>
    /// <param name="setting">形状の設定値</param>
    /// <param name="deltaCount">位置をずらすためのインデックス</param>
    private void SetSelectorShape(Selector selector, SelectorSetting setting, int deltaCount = 0)
    {
        selector.SetPosition(setting.position + setting.delta * deltaCount);
        selector.icon.sprite = setting.iconSpriteSample;
        SetRectSize(selector.iconRect, setting.iconSize);

        selector.previousButton.image.sprite = previousButtonSetting.sprite;
        selector.previousButtonRect.anchoredPosition = -setting.selectButtonDelta;
        SetRectSize(selector.previousButtonRect, previousButtonSetting.buttonSize);

        selector.nextButton.image.sprite = nextButtonSetting.sprite;
        selector.nextButtonRect.anchoredPosition = setting.selectButtonDelta;
        SetRectSize(selector.nextButtonRect, nextButtonSetting.buttonSize);
    }

    /// <summary>
    /// スプライトのアスペクト比に合わせてサイズを計算
    /// </summary>
    /// <param name="sprite">基準となるスプライト</param>
    /// <param name="originalSize">元のサイズ（計算結果で上書きされる）</param>
    /// <param name="sizeOption">サイズ調整のオプション</param>
    private void SetSpriteSize(Sprite sprite, ref Vector2 originalSize, SizeOption sizeOption)
    {
        Vector2 spriteSize = Vector2.one;
        if (sprite != null)
        {
            spriteSize = sprite.rect.size;
        }
        switch (sizeOption)
        {
            case SizeOption.None:
                break;
            case SizeOption.MatchAspectRatioBasedOnWidth:
                if (originalSize.x <= 0)
                {
                    Debug.LogWarning("幅の値が不正です。幅は正の値に設定してください。");
                    originalSize.x = 1e-3f;
                }
                originalSize.y = originalSize.x * spriteSize.y / spriteSize.x;
                break;
            case SizeOption.MatchAspectratioBasedOnHeight:
                if (originalSize.y <= 0)
                {
                    Debug.LogWarning("高さの値が不正です。高さは正の値に設定してください。");
                    originalSize.y = 1e-3f;
                }
                originalSize.x = originalSize.y * spriteSize.x / spriteSize.y;
                break;
        }
    }

    /// <summary>
    /// RectTransformのサイズを設定
    /// </summary>
    /// <param name="rectTransform">対象のRectTransform</param>
    /// <param name="size">設定するサイズ</param>
    private void SetRectSize(RectTransform rectTransform, Vector2 size)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    /// <summary>
    /// 指定したTransformの子オブジェクトを全て削除
    /// </summary>
    private void ClearChildren(Transform parent)
    {
        int loopCount = 0;
        while (parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
            loopCount++;
            if (loopCount > 1000)
            {
                Debug.LogError("The loop was too long and was terminated to prevent an infinite loop.");
                break;
            }
        }
    }
}
