using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChobinButton : MonoBehaviour
{
    [Header("追従対象のチョビン")]
    [SerializeField] private Transform targetChobin;
    [Header("コンポーネント参照")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private RectTransform buttonRect;

    public RectTransform ButtonRect => buttonRect;
    
    private Vector3 offset;

    private void Update()
    {
        Chase();
    }

    public void Init()
    {
        button=GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        buttonRect = button.GetComponent<RectTransform>();
    }

    /// <summary>
    /// ボタンを初期化し、追従対象とクリック時のアクションを設定します。
    /// </summary>
    /// <param name="chobinTransform">追従するチョビンのTransform</param>
    /// <param name="clickAction">クリック時に実行するアクション</param>
    public void Set(Transform chobinTransform, UnityAction clickAction)
    {
        targetChobin = chobinTransform;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(clickAction);
    }

    /// <summary>
    /// ボタンの見た目（スプライト、サイズ、オフセット）を設定します。
    /// </summary>
    /// <param name="sprite">表示するスプライト</param>
    /// <param name="size">ボタンのサイズ</param>
    /// <param name="newOffset">追従対象からのオフセット</param>
    public void SetAppearance(Sprite sprite, Vector2 size, Vector3 newOffset)
    {
        buttonImage.sprite = sprite;
        buttonRect.sizeDelta = size;
        offset = newOffset;
        Chase();
    }

    private void Chase()
    {
        if (targetChobin == null || buttonRect == null || Camera.main == null) return;
     Vector3 worldPos = targetChobin.position + offset;
buttonRect.position = worldPos;
    }

    /// <summary>
    /// カメラの回転に合わせてボタンの向きを調整します。
    /// </summary>
    /// <param name="angle">Y軸の回転角度</param>
    public void SetButtonDirection(float angle)
    {
        if (buttonRect != null)
            buttonRect.rotation = Quaternion.Euler(0, angle, 0);
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
