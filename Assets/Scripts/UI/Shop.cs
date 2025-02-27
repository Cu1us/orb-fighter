using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] RectTransform Container;
    [SerializeField] float VisibleX;

    [Header("Animation")]
    [SerializeField] float FadeInDuration;
    [SerializeField] Ease FadeInType;
    [SerializeField] float FadeOutDuration;
    [SerializeField] Ease FadeOutType;

    [Header("References")]
    [SerializeField] TextMeshProUGUI RoundCounter;
    [SerializeField] TextMeshProUGUI CurrencyCounter;
    [SerializeField] TextMeshProUGUI LivesCounter;

    void Start()
    {
        UpdateRoundCounter();
        UpdateLivesCounter();
    }
    public void UpdateRoundCounter()
    {
        RoundCounter.text = $"Round {GameManager.Round + 1}";
    }
    public void UpdateLivesCounter()
    {
        LivesCounter.text = GameManager.Instance.RemainingLives.ToString();
    }
    public void SetDisplayedCurrency(int currency)
    {
        CurrencyCounter.text = currency.ToString();
    }
    public void OnClickNextRound()
    {
        GameManager.Instance.TryStartNextRound();
    }
    public void FadeIn()
    {
        CancelInvoke(nameof(Hide));
        Show();
        Container.DOAnchorPosX(VisibleX, FadeInDuration).SetEase(FadeInType);
    }
    public void FadeOut()
    {
        Container.DOAnchorPosX(0, FadeOutDuration).SetEase(FadeOutType);
        Invoke(nameof(Hide), FadeOutDuration);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
