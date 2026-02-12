using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpriteSizeSetting
{
    public enum Option
    {
        MatchAspectRatioToWidth,
        MatchAspectRatioToHeight,
        NotMatchAspectRatio
    }

    private static float MatchedValue(float baseValue, float sizeX, float sizeY)
    {
        if (sizeX == 0f) return 0f;
        return baseValue * sizeY / sizeX;
    }

    public static void MatchAspectRaito(Sprite sprite, Option option, ref Vector2 size)
    {
        if (option == Option.NotMatchAspectRatio) return;
        Vector2 spriteSize = sprite != null ? sprite.rect.size : Vector2.one;
        Vector2 value = spriteSize;
        if (size != null)
        {
            value = size;
            switch (option)
            {
                case Option.MatchAspectRatioToWidth:
                    value.y = MatchedValue(value.x, spriteSize.x, spriteSize.y);
                    break;
                case Option.MatchAspectRatioToHeight:
                    value.x = MatchedValue(value.y, spriteSize.x, spriteSize.y);
                    break;
            }
        }
        size = value;
    }

    public static void SetSize(RectTransform rectTransform, Vector2 size)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }
}

[System.Serializable]
public class SelecterSetting
{
    [SerializeField] private Vector2 iconSize;
    [SerializeField] private SpriteSizeSetting.Option iconAspectRatioOption;
    [SerializeField] private Vector2 buttonSize;
    [SerializeField] private SpriteSizeSetting.Option buttonAspectRatioOption;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Vector2 buttonOffset;
    [SerializeField] private bool basedOnPreviousButton;

    public void MatchIconSize(Sprite sprite)
    {
        SpriteSizeSetting.MatchAspectRaito(
            sprite,
            iconAspectRatioOption,
            ref iconSize
            );
    }

    public void MatchButtonSize()
    {
        SpriteSizeSetting.MatchAspectRaito(
            buttonSprite,
            buttonAspectRatioOption,
            ref buttonSize
            );
        SpriteSizeSetting.MatchAspectRaito(
            buttonSprite,
            buttonAspectRatioOption,
            ref buttonSize
            );
    }

    public void SetIconTransform(RectTransform iconTransform, Sprite sprite = null)
    {
        if (sprite != null) MatchIconSize(sprite);
        SpriteSizeSetting.SetSize(iconTransform, iconSize);
    }

    public void SetButtonSprite(Image pButtonImage, Image nButtonImage)
    {
        pButtonImage.sprite = buttonSprite;
        nButtonImage.sprite = buttonSprite;
    }

    public void SetButtonTransform(RectTransform pButtonTransform, RectTransform nButtonTransform)
    {
        MatchButtonSize();
        SpriteSizeSetting.SetSize(pButtonTransform, buttonSize);
        SpriteSizeSetting.SetSize(nButtonTransform, buttonSize);
        float sign = basedOnPreviousButton ? 1f : -1f;
        pButtonTransform.anchoredPosition = sign * buttonOffset;
        nButtonTransform.anchoredPosition = -sign * buttonOffset;
    }

    public void ButtonFlip(RectTransform pButtonTransform, RectTransform nButtonTransform)
    {
        float value = basedOnPreviousButton ? 0f : 180f;
        pButtonTransform.localRotation = Quaternion.Euler(0f, 0f, value);
        nButtonTransform.localRotation = Quaternion.Euler(0f, 0f, 180f - value);
    }
}

public class Selecter : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private SelecterSetting setting;

    private Sprite[] icons;
    private int currentIndex;
    private UnityAction<int> onSelect;
    private RectTransform iconTransform;
    private RectTransform previousButtonTransform;
    private RectTransform nextButtonTransform;

    private int elementLength => icons.Length;

    public bool CheckSettings()
    {
        if (iconImage == null)
        {
            Debug.LogError("Selecter: Icon image is not assigned.");
            return false;
        }
        if (previousButton == null)
        {
            Debug.LogError("Selecter: Previous button is not assigned.");
            return false;
        }
        if (nextButton == null)
        {
            Debug.LogError("Selecter: Next button is not assigned.");
            return false;
        }
        if (setting == null)
        {
            Debug.LogError("Selecter: Setting is not assigned.");
            return false;
        }
        return true;
    }

    public void SetSettings()
    {
        if (iconTransform == null)
        {
            iconTransform = (RectTransform)iconImage.transform;
        }
        if (previousButtonTransform == null)
        {
            previousButtonTransform = (RectTransform)previousButton.transform;
        }
        if (nextButtonTransform == null)
        {
            nextButtonTransform = (RectTransform)nextButton.transform;
        }

        setting.SetIconTransform(iconTransform);
        setting.SetButtonSprite(previousButton.image, nextButton.image);
        setting.SetButtonTransform(previousButtonTransform, nextButtonTransform);
        setting.ButtonFlip(previousButtonTransform, nextButtonTransform);
    }

    public void SetSettings(SelecterSetting _setting)
    {
        setting = _setting;
        SetSettings();
    }

    public void SetIcons(Sprite[] _icons, int _currentIndex = 0)
    {
        icons = _icons;
        currentIndex = _currentIndex;
        iconImage.sprite = icons[currentIndex];
    }

    public void SetAction(UnityAction<int> _onSelect)
    {
        onSelect = _onSelect;
        previousButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();
        previousButton.onClick.AddListener(
            () => SetIndex(currentIndex - 1)
        );
        nextButton.onClick.AddListener(
            () => SetIndex(currentIndex + 1)
        );
    }

    private void SetIndex(int index)
    {
        while (index < 0) index += elementLength;
        while (index >= elementLength) index -= elementLength;
        currentIndex = index;
        iconImage.sprite = icons[currentIndex];
        setting.SetIconTransform(iconTransform, iconImage.sprite);
        onSelect?.Invoke(currentIndex);
    }

    public void ResetSelection()
    {
        SetIndex(0);
    }
}
