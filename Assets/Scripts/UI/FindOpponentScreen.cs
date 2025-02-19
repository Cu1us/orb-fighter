using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FindOpponentScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] Image Background;

    [Header("Animation")]
    [SerializeField] float FadeInDuration;
    [SerializeField] Ease FadeInType;
    [SerializeField] float FadeOutDuration;
    [SerializeField] Ease FadeOutType;

    // Local vars
    float defaultBackgroundAlpha;
    float defaultLoadingTextAlpha;


    void Start()
    {
        defaultBackgroundAlpha = Background.color.a;
        defaultLoadingTextAlpha = LoadingText.color.a;
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0);
        LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, 0);
    }

    public void OnStartSearch()
    {
        LoadingText.text = "Finding opponent...";
        FadeIn();
    }
    public void OnFoundTeam(SerializableTeam team)
    {
        LoadingText.text = "Found opponent!";
        FadeOut();
    }
    public void FadeIn()
    {
        CancelInvoke(nameof(Hide));
        Show();
        Background.DOFade(defaultBackgroundAlpha, FadeInDuration).SetEase(FadeInType);
        LoadingText.DOFade(defaultLoadingTextAlpha, FadeInDuration).SetEase(FadeInType);
    }
    public void FadeOut()
    {
        Background.DOFade(0, FadeOutDuration).SetEase(FadeOutType);
        LoadingText.DOFade(0, FadeOutDuration).SetEase(FadeOutType);
        Invoke(nameof(Hide), FadeOutDuration);
    }
    public void Show()
    {
        Background.gameObject.SetActive(true);
    }
    public void Hide()
    {
        Background.gameObject.SetActive(false);
    }
}
