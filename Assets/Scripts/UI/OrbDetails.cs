using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OrbDetails : MonoBehaviour
{
    [SerializeField] RectTransform container;
    [SerializeField] Button closeButton;


    [SerializeField] float openCloseAnimationDuration;
    [SerializeField] AnimationCurve openCloseCurve;

    [Header("Info blocks")]
    [SerializeField] UpgradeInfoBlock infoBlockPrefab;
    [SerializeField] Transform infoBlockContainer;

    public List<UpgradeInfoBlock> spawnedInfoBlocks;
    ObjectPool<UpgradeInfoBlock> Pool;

    public OrbSpawner DisplayedSpawner;

    public float viewportXMaxToCountClickAsOutsideMenu;

    bool toBeHidden;
    bool isHidden;
    bool closeButtonPressed;
    float animationProgress;

    #region InfoBlock pool
    void SetupObjectPool()
    {
        Pool = new(
            createFunc: InstantiateInfoBlock,
            actionOnGet: (block) =>
            {
                spawnedInfoBlocks.Add(block);
                block.gameObject.SetActive(true);
            },
            actionOnRelease: (block) =>
            {
                spawnedInfoBlocks.Remove(block);
                block.ClearData();
                block.gameObject.SetActive(false);
            },
            actionOnDestroy: (a) => Destroy(a.gameObject)
        );
    }

    UpgradeInfoBlock InstantiateInfoBlock()
    {
        UpgradeInfoBlock instance = Instantiate(infoBlockPrefab, infoBlockContainer);
        instance.ClearData();
        return instance;
    }

    UpgradeInfoBlock AddInfoBlockToUI(Upgrade upgradeToDisplay, int upgradeLevel = 0)
    {
        UpgradeInfoBlock block = Pool.Get();
        block.SetData(upgradeToDisplay, upgradeLevel);
        return block;
    }

    void ClearInfoBlocks()
    {
        foreach (UpgradeInfoBlock block in spawnedInfoBlocks.ToArray())
        {
            Pool.Release(block);
        }
        spawnedInfoBlocks.Clear();
    }
    #endregion

    void Awake()
    {
        SetupObjectPool();
    }
    void Start()
    {
        toBeHidden = true;
        isHidden = true;
    }

    public void OnOrbSpawnerClicked(OrbSpawner spawner)
    {
        if (spawner == DisplayedSpawner)
        {
            SetHidden(!toBeHidden);
        }
        else if (toBeHidden)
        {
            SetHidden(false);
        }
        DisplayOrbData(spawner);
    }

    void DisplayOrbData(OrbSpawner spawner)
    {
        DisplayedSpawner = spawner;
        ClearInfoBlocks();
        foreach (KeyValuePair<Upgrade, int> upgrade in spawner.GetUpgrades())
        {
            AddInfoBlockToUI(upgrade.Key, upgrade.Value);
        }
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
        if (DetectClickOutsideMenu() && !toBeHidden)
        {
            SetHidden(true);
        }
    }

    bool DetectClickOutsideMenu()
    {
        return Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject();
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

        bool menuHidden = container.anchoredPosition.x >= 0;
        if (menuHidden && !isHidden) // Just became hidden
        {
            ClearInfoBlocks();
        }
        isHidden = menuHidden;

        bool closeButtonVisible = !menuHidden && !closeButtonPressed;
        if (closeButton.gameObject.activeSelf != closeButtonVisible)
        {
            closeButton.gameObject.SetActive(closeButtonVisible);
        }
    }

    public void SetHidden(bool hidden)
    {
        toBeHidden = hidden;
        if (!hidden) closeButtonPressed = false;
    }
}
