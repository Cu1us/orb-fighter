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

    [SerializeField] float openCloseAnimationDuration;
    [SerializeField] AnimationCurve openCloseCurve;

    public OrbSpawner DisplayedSpawner;

    bool toBeHidden;
    bool closeButtonPressed;
    float animationProgress;

    void Start()
    {
        toBeHidden = true;
    }

    public void OnOrbSpawnerClicked(OrbSpawner spawner)
    {
        SetHidden(!toBeHidden);
    }

    public void OnCloseButtonClicked()
    {
        closeButtonPressed = true;
        SetHidden(true);
    }

    void Update()
    {
        AnimateOpenClose();
        if (DisplayedSpawner && !toBeHidden)
        {
            DisplayedSpawner.Highlight(Color.cyan);
        }
    }

    void AnimateOpenClose()
    {
        float targetX = toBeHidden ? 0 : container.sizeDelta.x;

        if (container.anchoredPosition.x != targetX)
        {
            float progressChange = Time.deltaTime / openCloseAnimationDuration;
            if (toBeHidden) progressChange = -progressChange;
            if (closeButtonPressed) progressChange *= 2;
            animationProgress += progressChange;
            animationProgress = Mathf.Clamp01(animationProgress);

            float newX = -container.sizeDelta.x * openCloseCurve.Evaluate(animationProgress);
            container.anchoredPosition = new Vector2(newX, container.anchoredPosition.y);
        }

        bool closeButtonVisible = container.anchoredPosition.x < 0 && !closeButtonPressed;
        if (closeButton.gameObject.activeSelf != closeButtonVisible) closeButton.gameObject.SetActive(closeButtonVisible);
    }

    public void SetHidden(bool hidden)
    {
        toBeHidden = hidden;
        if (!hidden) closeButtonPressed = false;
    }

    public void ShowInfo(OrbSpawner spawner)
    {

    }
}
