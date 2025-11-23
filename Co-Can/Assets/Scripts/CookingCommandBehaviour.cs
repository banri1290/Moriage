//======================================================================
// CookingCommandBehaviour.cs
// 調理コマンドのUIを制御するMonoBehaviour
// 各コマンドの材料・アクションのテキストとボタンの配置・制御を管理します。
//======================================================================

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class CookingCommandBehaviour : GameSystem
{
    enum SpriteSizeOption
    {
        NonKeepAspect, // アスペクト比を維持しない
        KeepAspectWithCurrentWidth, // 現在の幅を維持してアスペクト比を維持
        KeepAspectWithCurrentHeight, // 現在の高さを維持してアスペクト比を維持
    }

    public class SelectCommandEvent : UnityEvent<int, int> { }

    [Header("UIモード設定")]
    [Tooltip("true: 画像で表示, false: テキストで表示")]
    [SerializeField] private bool useImageUI = true;

    [Header("UIオブジェクト参照")]
    [Tooltip("材料/調理法選択UIの1行分のプレハブ")]
    [SerializeField] private GameObject commandUIPrefab;
    [Tooltip("指示UI全体の親となるCanvasオブジェクト")]
    [SerializeField] private GameObject commandCanvas;
    [Tooltip("生成したUIプレハブを格納する親オブジェクト")]
    [SerializeField] private GameObject commandUIParent;

    [Header("UI生成設定")]
    [Tooltip("生成される材料UIオブジェクトの基本名")]
    [SerializeField] private string materialUIPrefabName = "MaterialUI";
    [Tooltip("生成される調理法UIオブジェクトの基本名")]
    [SerializeField] private string actionUIPrefabName = "ActionUI";
    [Tooltip("エディタ上で（再生中以外で）指示UIを常に表示しておくか")]
    [SerializeField] private bool commandCanvasIsActiveInEditor = false;

    [Header("UIレイアウト設定")]
    [Tooltip("表示する指示の行数")]
    [SerializeField] private int commandCount = 3;
    [Tooltip("1行目のUIの初期位置")]
    [SerializeField] private Vector2 commandPosition;
    [Tooltip("各UI要素の間隔（X: 材料と調理法の間, Y: 行間）")]
    [SerializeField] private Vector2 commandDelta;
    [Tooltip("UI内の左ボタンの相対位置")]
    [SerializeField] private Vector2 commandLeftButtonPosition;
    [Tooltip("UI内の右ボタンの相対位置")]
    [SerializeField] private Vector2 commandRightButtonPosition;

    [Header("画像設定 (useImageUI = true)")]
    [Tooltip("UI内の画像の相対位置")]
    [SerializeField] private Vector2 commandImagePosition;
    [Tooltip("画像表示領域のサイズ")]
    [SerializeField] private Vector2 imageSize = new Vector2(100, 100);
    [SerializeField] private SpriteSizeOption UIImageSizeOption = SpriteSizeOption.NonKeepAspect;

    [Header("フォント設定 (useImageUI = false)")]
    [Tooltip("UI内のテキストの相対位置")]
    [SerializeField] private Vector2 commandTextPosition;
    [Tooltip("UIテキストに使用するフォントアセット")]
    [SerializeField] private TMP_FontAsset fontAsset;
    [Tooltip("フォントの色")]
    [SerializeField] private Color fontColor = Color.white;
    [Tooltip("フォントのサイズ")]
    [SerializeField] private float fontSize = 16f;
    [Tooltip("テキスト表示領域のサイズ")]
    [SerializeField] private Vector2 textSize = new Vector2(200, 30);


    [Header("ボタン設定")]
    [Tooltip("左矢印ボタンの画像")]
    [SerializeField] private Sprite leftButtonSprite;
    [Tooltip("右矢印ボタンの画像")]
    [SerializeField] private Sprite rightButtonSprite;
    [Tooltip("ボタンの基本サイズ")]
    [SerializeField] private Vector2 ButtonSize = new Vector2(100, 100);
    [Tooltip("ボタン画像のサイズ調整方法")]
    [SerializeField] private SpriteSizeOption buttonSizeOption = SpriteSizeOption.NonKeepAspect;

    // UIコンポーネントの参照
    private RectTransform[] materialUIRects;
    private Image[] materialUIImages;
    private TextMeshProUGUI[] materialUITexts;
    private Button[] materialLeftButtons;
    private Button[] materialRightButtons;
    private RectTransform[] actionUIRects;
    private Image[] actionUIImages;
    private TextMeshProUGUI[] actionUITexts;
    private Button[] actionLeftButtons;
    private Button[] actionRightButtons;

    private SelectCommandEvent previousMaterialEvent = new();
    private SelectCommandEvent nextMaterialEvent = new();
    private SelectCommandEvent previousActionEvent = new();
    private SelectCommandEvent nextActionEvent = new();
    private UnityEvent submitCommandEvent = new();

    private int currentChobinUIIndex;

    public int CommandCount => commandCount; // 指示の行数
    public int CurrentChobinUIIndex => currentChobinUIIndex;
    public SelectCommandEvent PreviousMaterialEvent => previousMaterialEvent;
    public SelectCommandEvent NextMaterialEvent => nextMaterialEvent;
    public SelectCommandEvent PreviousActionEvent => previousActionEvent;
    public SelectCommandEvent NextActionEvent => nextActionEvent;
    public UnityEvent SubmitCommandEvent => submitCommandEvent;

    // 初期化処理
    void Start()
    {
        currentChobinUIIndex = 0;
    }

    // 毎フレームの更新処理（現在未使用）
    void Update()
    {

    }

    /// <summary>
    /// 設定の検証とUIの初期化
    /// </summary>
    public override bool CheckSettings()
    {
        bool commandUIisCorrect = true;

        // UIの行数を補正
        if (commandCount < 1)
        {
            commandCount = 1;
        }
        // ボタンの座標を補正
        if (commandLeftButtonPosition.x >= 0)
        {
            commandLeftButtonPosition.x = -1e-5f; // 左ボタンの位置が右側にならないようにする
            Debug.LogWarning("左ボタンの位置が右側に設定されています。左ボタンは中央より左に配置してください。");
        }
        if (commandRightButtonPosition.x <= 0)
        {
            commandRightButtonPosition.x = 1e-5f; // 右ボタンの位置が左側にならないようにする
            Debug.LogWarning("右ボタンの位置が左側に設定されています。右ボタンは中央より右に配置してください。");
        }

        if (commandCanvas == null)
        {
            commandUIisCorrect = false;
            Debug.LogError("指示UIを制御するCanvasオブジェクトが設定されていません。");
        }

        if (commandUIParent == null)
        {
            commandUIisCorrect = false;
            Debug.LogError("コマンドUIの親オブジェクトが設定されていません。");
        }

        // 材料・調理法オブジェクトがnullでないか確認
        if (commandUIPrefab != null)
        {
            bool hasTextComponent = false;
            bool hasImageComponent = false;
            bool hasLeftButton = false;
            bool hasRightButton = false;
            // 材料オブジェクトの子要素をチェック
            foreach (Transform child in commandUIPrefab.transform)
            {
                if (child.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    if (child.GetComponent<TextMeshProUGUI>())
                    {
                        hasTextComponent = true;
                    }
                    else if (child.GetComponent<Image>() && child.GetComponent<Button>() == null)
                    {
                        hasImageComponent = true;
                    }
                    else if (child.GetComponent<Button>())
                    {
                        if (rectTransform.anchoredPosition.x < 0)
                        {
                            hasLeftButton = true;
                        }
                        else
                        {
                            hasRightButton = true;
                        }
                    }
                }
            }
            // 必要なUI要素が揃っているか確認
            if (hasTextComponent && hasImageComponent && hasLeftButton && hasRightButton)
            {
            }
            else
            {
                commandUIisCorrect = false;
                if (!hasTextComponent)
                {
                    Debug.LogError("コマンドUIプレハブにテキストコンポーネントが見つかりません。");
                }
                if (!hasImageComponent)
                {
                    Debug.LogError("コマンドUIプレハブに画像コンポーネントが見つかりません。");
                }
                if (!hasLeftButton)
                {
                    Debug.LogError("コマンドUIプレハブに左ボタンとなるButtonコンポーネントが見つかりません。");
                }
                if (!hasRightButton)
                {
                    Debug.LogError("コマンドUIプレハブに右ボタンとなるButtonコンポーネントが見つかりません。");
                }
            }
        }
        else
        {
            commandUIisCorrect = false;
            Debug.LogError("コマンドUIプレハブが設定されていません。");
        }

        return commandUIisCorrect;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            commandCanvas.SetActive(false);

            InitUIParent();
        }
        else
        {
            commandCanvas.SetActive(commandCanvasIsActiveInEditor);

            bool changeCommandCount = (commandUIParent.transform.childCount != commandCount * 2);
            bool changeMaterialUIName = (commandUIParent.transform.GetChild(0).name != materialUIPrefabName + "_0");
            bool changeActionUIName = (commandUIParent.transform.GetChild(1).name != actionUIPrefabName + "_0");
            if (changeCommandCount || changeMaterialUIName || changeActionUIName)
            {
                Debug.Log("コマンド数が変更されたため、UIを再生成します。");
                EditorApplication.delayCall += InitUIParent; // UIの親オブジェクトを初期化
            }
            else
            {
                InitUI(); // UIのスタイルを更新
            }
        }
#else
        commandCanvas.SetActive(false);
        InitUIParent();
#endif

        Debug.Log("CookingCommandBehaviourの初期化が正常に完了しました。");
    }

    private void InitUIParent()
    {
        materialUIRects = new RectTransform[commandCount];
        actionUIRects = new RectTransform[commandCount];
        materialUIImages = new Image[commandCount];
        materialUITexts = new TextMeshProUGUI[commandCount];
        materialLeftButtons = new Button[commandCount];
        materialRightButtons = new Button[commandCount];
        actionUIImages = new Image[commandCount];
        actionUITexts = new TextMeshProUGUI[commandCount];
        actionLeftButtons = new Button[commandCount];
        actionRightButtons = new Button[commandCount];

        // 既存のUIオブジェクトを削除
        while (commandUIParent.transform.childCount > 0)
        {
            DestroyImmediate(commandUIParent.transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < commandCount; i++)
        {
            GameObject materialUIObject = null;
            GameObject actionUIObject = null;
#if UNITY_EDITOR
            materialUIObject = UnityEditor.PrefabUtility.InstantiatePrefab(commandUIPrefab, commandUIParent.transform) as GameObject;
            actionUIObject = UnityEditor.PrefabUtility.InstantiatePrefab(commandUIPrefab, commandUIParent.transform) as GameObject;
#else
            materialUIObject = Instantiate(commandUIPrefab, commandUIParent.transform);
            actionUIObject = Instantiate(commandUIPrefab, commandUIParent.transform);
#endif
            materialUIObject.name = materialUIPrefabName + "_" + i; // 材料UIの名前を設定
            actionUIObject.name = actionUIPrefabName + "_" + i; // 調理法UIの名前を設定

            // UIの位置を設定
            materialUIRects[i] = materialUIObject.GetComponent<RectTransform>();
            actionUIRects[i] = actionUIObject.GetComponent<RectTransform>();

            foreach (Transform t in materialUIObject.transform)
            {
                if (t.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    if (t.GetComponent<Image>() != null && t.GetComponent<Button>() == null)
                    {
                        materialUIImages[i] = t.GetComponent<Image>();
                    }
                    else if (t.GetComponent<TextMeshProUGUI>() != null)
                    {
                        materialUITexts[i] = t.GetComponent<TextMeshProUGUI>();
                    }
                    else if (t.GetComponent<Button>() != null)
                    {
                        if (rectTransform.anchoredPosition.x < 0)
                        {
                            materialLeftButtons[i] = t.GetComponent<Button>();
                        }
                        else
                        {
                            materialRightButtons[i] = t.GetComponent<Button>();
                        }
                    }
                }
            }
            foreach (Transform t in actionUIObject.transform)
            {
                if (t.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    if (t.GetComponent<Image>() != null && t.GetComponent<Button>() == null)
                    {
                        actionUIImages[i] = t.GetComponent<Image>();
                    }
                    else if (t.GetComponent<TextMeshProUGUI>() != null)
                    {
                        actionUITexts[i] = t.GetComponent<TextMeshProUGUI>();
                    }
                    else if (t.GetComponent<Button>() != null)
                    {
                        if (rectTransform.anchoredPosition.x < 0)
                        {
                            actionLeftButtons[i] = t.GetComponent<Button>();
                        }
                        else
                        {
                            actionRightButtons[i] = t.GetComponent<Button>();
                        }
                    }
                }
            }
        }
        InitUI();
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.delayCall -= InitUIParent; // 重複して実行されるのを防ぐために削除
        }
#endif
    }

    private void InitUI()
    {
#if UNITY_EDITOR
        bool recallInitUIParent = false;
        if (materialUIRects == null)
        {
            recallInitUIParent = true;
        }
        else if (materialUIRects.Length != commandCount)
        {
            recallInitUIParent = true;
        }

        if (recallInitUIParent)
        {
            Debug.LogWarning("UIコンポーネントの配列が初期化されていないため、コマンドを再生成します。UIの親オブジェクトを再初期化します。");
            EditorApplication.delayCall += InitUIParent; // UIの親オブジェクトを初期化
            return;
        }
#endif
        imageSize = SpriteSize(materialUIImages[0].sprite, imageSize, UIImageSizeOption);
        //ボタンのサイズオプションに応じてサイズを補正
        ButtonSize = SpriteSize(leftButtonSprite, ButtonSize, buttonSizeOption);

        // UIの配置とスタイル設定
        for (int i = 0; i < commandCount; i++)
        {
            materialUIRects[i].anchoredPosition = commandPosition + new Vector2(0, -commandDelta.y * i);
            actionUIRects[i].anchoredPosition = commandPosition + new Vector2(commandDelta.x, -commandDelta.y * i);

            Button materialLeftButton = materialLeftButtons[i];
            Button materialRightButton = materialRightButtons[i];
            Button actionLeftButton = actionLeftButtons[i];
            Button actionRightButton = actionRightButtons[i];

            {
                // 画像のスタイル設定
                materialUIImages[i].rectTransform.anchoredPosition = commandImagePosition;
                materialUIImages[i].rectTransform.sizeDelta = imageSize;
                actionUIImages[i].rectTransform.anchoredPosition = commandImagePosition;
                actionUIImages[i].rectTransform.sizeDelta = imageSize;
            }
            {
                // テキストのスタイル設定
                materialUITexts[i].rectTransform.anchoredPosition = commandTextPosition;
                materialUITexts[i].rectTransform.sizeDelta = textSize;
                materialUITexts[i].font = fontAsset;
                materialUITexts[i].color = fontColor;
                materialUITexts[i].fontSize = fontSize;
                actionUITexts[i].rectTransform.anchoredPosition = commandTextPosition;
                actionUITexts[i].rectTransform.sizeDelta = textSize;
                actionUITexts[i].font = fontAsset;
                actionUITexts[i].color = fontColor;
                actionUITexts[i].fontSize = fontSize;
            }
            // ボタンのスタイル設定
            materialLeftButton.GetComponent<RectTransform>().anchoredPosition = commandLeftButtonPosition;
            materialLeftButton.GetComponent<RectTransform>().sizeDelta = ButtonSize;
            Image materialLeftButtonImage = materialLeftButton.GetComponent<Image>();
            if (materialLeftButtonImage != null && leftButtonSprite != null)
            {
                materialLeftButtonImage.sprite = leftButtonSprite;
            }
            materialRightButton.GetComponent<RectTransform>().anchoredPosition = commandRightButtonPosition;
            materialRightButton.GetComponent<RectTransform>().sizeDelta = ButtonSize;
            Image materialRightButtonImage = materialRightButton.GetComponent<Image>();
            if (materialRightButtonImage != null && rightButtonSprite != null)
            {
                materialRightButtonImage.sprite = rightButtonSprite;
            }
            actionLeftButton.GetComponent<RectTransform>().anchoredPosition = commandLeftButtonPosition;
            actionLeftButton.GetComponent<RectTransform>().sizeDelta = ButtonSize;
            Image actionLeftButtonImage = actionLeftButton.GetComponent<Image>();
            if (actionLeftButtonImage != null && leftButtonSprite != null)
            {
                actionLeftButtonImage.sprite = leftButtonSprite;
            }
            actionRightButton.GetComponent<RectTransform>().anchoredPosition = commandRightButtonPosition;
            actionRightButton.GetComponent<RectTransform>().sizeDelta = ButtonSize;
            Image actionRightButtonImage = actionRightButton.GetComponent<Image>();
            if (actionRightButtonImage != null && rightButtonSprite != null)
            {
                actionRightButtonImage.sprite = rightButtonSprite;
            }

            materialUITexts[i].gameObject.SetActive(!useImageUI);
            actionUITexts[i].gameObject.SetActive(!useImageUI);
            materialUIImages[i].gameObject.SetActive(useImageUI);
            actionUIImages[i].gameObject.SetActive(useImageUI);

            // ボタンのクリックイベントを設定
            int commandIndex = i; // ローカル変数を使用してクロージャーの問題を回避
            materialLeftButton.onClick.RemoveAllListeners();
            materialRightButton.onClick.RemoveAllListeners();
            actionLeftButton.onClick.RemoveAllListeners();
            actionRightButton.onClick.RemoveAllListeners();

            materialLeftButton.onClick.AddListener(() => previousMaterialEvent.Invoke(currentChobinUIIndex, commandIndex));
            materialRightButton.onClick.AddListener(() => nextMaterialEvent.Invoke(currentChobinUIIndex, commandIndex));
            actionLeftButton.onClick.AddListener(() => previousActionEvent.Invoke(currentChobinUIIndex, commandIndex));
            actionRightButton.onClick.AddListener(() => nextActionEvent.Invoke(currentChobinUIIndex, commandIndex));
        }
    }

    /// <summary>
    /// 指定されたインデックスの材料UIを更新します。
    /// </summary>
    public void UpdateMaterial(int commandIndex, Material material)
    {
        if (useImageUI)
        {
            UpdateMaterialImage(commandIndex, material.Sprite);
        }
        else
        {
            UpdateMaterialText(commandIndex, material.Name);
        }
    }

    /// <summary>
    /// 指定されたインデックスの調理法UIを更新します。
    /// </summary>
    public void UpdateAction(int commandIndex, Action action)
    {
        if (useImageUI)
        {
            UpdateActionImage(commandIndex, action.Sprite);
        }
        else
        {
            UpdateActionText(commandIndex, action.Name);
        }
    }

    private void UpdateMaterialImage(int commandIndex, Sprite sprite)
    {
        if (commandIndex < 0 || commandIndex >= materialUIImages.Length) return;
        materialUIImages[commandIndex].sprite = sprite;
    }

    private void UpdateActionImage(int commandIndex, Sprite sprite)
    {
        if (commandIndex < 0 || commandIndex >= actionUIImages.Length) return;
        actionUIImages[commandIndex].sprite = sprite;
    }

    private void UpdateMaterialText(int commandIndex, string name)
    {
        if (commandIndex < 0 || commandIndex >= materialUITexts.Length) return;
        materialUITexts[commandIndex].text = name;
    }

    private void UpdateActionText(int commandIndex, string name)
    {
        if (commandIndex < 0 || commandIndex >= actionUITexts.Length) return;
        actionUITexts[commandIndex].text = name;
    }

    private Vector2 SpriteSize(Sprite sprite, Vector2 targetSize, SpriteSizeOption spriteSizeOption)
    {
        Vector2 newSize = targetSize;
        switch (spriteSizeOption)
        {
            case SpriteSizeOption.NonKeepAspect:
                // そのまま
                break;
            case SpriteSizeOption.KeepAspectWithCurrentWidth:
                if (sprite != null)
                {
                    float aspectRatio = sprite.rect.height / sprite.rect.width;
                    newSize.y = newSize.x * aspectRatio;
                }
                break;
            case SpriteSizeOption.KeepAspectWithCurrentHeight:
                if (sprite != null)
                {
                    float aspectRatio = sprite.rect.width / sprite.rect.height;
                    newSize.x = newSize.y * aspectRatio;
                }
                break;
        }
        return newSize;
    }

    public void ShowCommand(int chobinIndex)
    {
        commandCanvas.SetActive(true);
        currentChobinUIIndex = chobinIndex;
    }

    public void SubmitCommand()
    {
        submitCommandEvent.Invoke();
        commandCanvas.SetActive(false);
    }
}