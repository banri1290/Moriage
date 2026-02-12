using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    enum FadeState
    {
        None,
        FadeOut,
        FadeIn
    }
    enum FadeType
    {
        MakeColorBlack,
        MakeColorWhite,
        MakeAlphaZero,
        HideByThisImageInBlack,
        HideByThisImageInWhite
    }

    [SerializeField] private Image targetImage;
    [SerializeField] private float timeToFade = 1f;
    [SerializeField] private FadeState fadeStateOnAwake = FadeState.None;
    [SerializeField] private FadeType fadeType = FadeType.MakeAlphaZero;
    [SerializeField] private UnityEvent onCompleteFade;
    [SerializeField] private bool continueActiveOnFadeOutComplete = false;

    private FadeState currentFadeState = FadeState.None;

    private FlagTimer fadeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogError("Fade: No Image component found for fading.");
            return;
        }
        fadeTimer = new FlagTimer(this,timeToFade);
        fadeTimer.SetListenerOnUpdate(SetColor);
        SetFadeState(fadeStateOnAwake);
    }

    public void FadeOut()
    {
        targetImage.gameObject.SetActive(true);
        SetFadeState(FadeState.FadeOut);
    }

    public void FadeIn()
    {
        targetImage.gameObject.SetActive(true);
        SetFadeState(FadeState.FadeIn);
    }

    void SetFadeState(FadeState state)
    {
        currentFadeState = state;
        switch (currentFadeState)
        {
            case FadeState.FadeOut:
                fadeTimer.SetListenerOnComplete(CompleteFade);
                fadeTimer.Set();
                break;
            case FadeState.FadeIn:
                fadeTimer.SetListenerOnComplete(CompleteFade);
                fadeTimer.Set();
                break;
            case FadeState.None:
            default:
                break;
        }
        SetColor(0f);
    }

    void CompleteFade()
    {
        SetColor(1f);
        onCompleteFade?.Invoke();
        bool HideByThisImage
        = fadeType == FadeType.HideByThisImageInBlack || fadeType == FadeType.HideByThisImageInWhite;
        if (!continueActiveOnFadeOutComplete
            && (currentFadeState == FadeState.FadeOut && !HideByThisImage)
            || (currentFadeState == FadeState.FadeIn && HideByThisImage))
        {
            targetImage.gameObject.SetActive(false);
        }
        currentFadeState = FadeState.None;
    }

    void SetColor(float value)
    {
        if (currentFadeState == FadeState.None)
        {
            return;
        }
        float colorValue = currentFadeState == FadeState.FadeIn ? value : 1f - value;
        switch (fadeType)
        {
            case FadeType.MakeColorBlack:
                targetImage.color = new Color(colorValue, colorValue, colorValue, 1f);
                break;
            case FadeType.MakeColorWhite:
                targetImage.color = new Color(1 - colorValue, 1 - colorValue, 1 - colorValue, 1f);
                break;
            case FadeType.MakeAlphaZero:
                targetImage.color = new Color(1f, 1f, 1f, colorValue);
                break;
            case FadeType.HideByThisImageInBlack:
                targetImage.color = new Color(0f, 0f, 0f, 1f - colorValue);
                break;
            case FadeType.HideByThisImageInWhite:
                targetImage.color = new Color(1f, 1f, 1f, 1f - colorValue);
                break;
        }
    }
}
