using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OrbDetails : MonoBehaviour
{
    [SerializeField] RectTransform container;
    [SerializeField] Button closeButton;

    [Header("Tween Settings")]
    [SerializeField] float tweenDuration;

    bool hidden;

    public void OnOrbSpawnerClicked(OrbSpawner spawner)
    {
        Debug.Log("Click detected!");
        if (hidden) TweenIn(); else TweenOut();
    }

    public void TweenIn()
    {
        closeButton.gameObject.SetActive(true);
        SetHidden(false);
        container.DOAnchorPosX(-container.sizeDelta.x, tweenDuration).OnComplete(OnTweenInFinished);
    }
    void OnTweenInFinished()
    {

    }
    public void TweenOut()
    {
        closeButton.gameObject.SetActive(false);
        container.DOAnchorPosX(0, tweenDuration).OnComplete(OnTweenOutFinished);
    }
    void OnTweenOutFinished()
    {
        SetHidden(true);
    }

    public void SetHidden(bool hidden)
    {
        this.hidden = hidden;
        container.gameObject.SetActive(!hidden);
    }
}
