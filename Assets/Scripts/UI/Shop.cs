using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    [SerializeField] RectTransform Container;
    [SerializeField] float VisibleX;

    [Header("Animation")]
    [SerializeField] float FadeInDuration;
    [SerializeField] Ease FadeInType;
    [SerializeField] float FadeOutDuration;
    [SerializeField] Ease FadeOutType;

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
