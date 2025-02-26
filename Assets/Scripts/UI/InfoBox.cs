using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoBox : MonoBehaviour, IPointerClickHandler, IDeselectHandler
{
    private static InfoBox _instance;
    public static InfoBox Instance
    {
        get
        {
            if (_instance) return _instance;
            else throw new NullReferenceException("A script attempted to access the static InfoBox instance, but it has not yet been initialized.");
        }
    }


    [SerializeField] RectTransform MainContainer;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI Description;

    [SerializeField] GameObject StatsContainer;
    [SerializeField] TextMeshProUGUI StatsHeader;
    [SerializeField] GameObject HealthBonusContainer;
    [SerializeField] GameObject AttackBonusContainer;
    [SerializeField] TextMeshProUGUI HealthBonus;
    [SerializeField] TextMeshProUGUI AttackBonus;

    bool isVisible;
    public Guid currentBoxGuid { get; private set; }


    void Awake()
    {
        if (!_instance) _instance = this;
    }

    public Guid ShowUpgrade(Upgrade upgrade, Vector2 anchoredPosition, bool showStats = true)
    {
        if (showStats && upgrade.AddStats)
        {
            AttackBonusContainer.SetActive(upgrade.AttackDamageIncrease != 0);
            HealthBonusContainer.SetActive(upgrade.MaxHealthIncrease != 0);
            AttackBonus.text = upgrade.AttackDamageIncrease < 0 ? upgrade.AttackDamageIncrease.ToString() : $"+{upgrade.AttackDamageIncrease}";
            HealthBonus.text = upgrade.MaxHealthIncrease < 0 ? upgrade.MaxHealthIncrease.ToString() : $"+{upgrade.MaxHealthIncrease}";

            StatsContainer.SetActive(true);
        }
        else
        {
            StatsContainer.SetActive(false);
        }

        Title.text = upgrade.Name;
        Description.text = upgrade.Description;
        StatsHeader.text = "When applied onto orb:";

        Guid guid = Show(anchoredPosition);
        return guid;
    }
    public Guid ShowOrbType(OrbType type, Vector2 anchoredPosition)
    {
        Title.text = type.Name;
        Description.text = type.Description;
        StatsHeader.text = "Orb stats:";

        AttackBonus.text = type.StartingAttackDamage.ToString();
        HealthBonus.text = type.StartingHealth.ToString();

        StatsContainer.SetActive(true);
        AttackBonusContainer.SetActive(true);
        HealthBonusContainer.SetActive(true);

        Guid guid = Show(anchoredPosition);
        return guid;
    }

    public Guid Show(Vector2 anchoredPosition, Vector2 pivot)
    {
        currentBoxGuid = Guid.NewGuid();
        SetPosition(anchoredPosition, pivot);
        SetVisible(true);
        MarkSelected();
        return currentBoxGuid;
    }
    public Guid Show(Vector2 anchoredPosition)
    {
        currentBoxGuid = Guid.NewGuid();
        SetPosition(anchoredPosition, Vector2.one);
        SetVisible(true);
        MarkSelected();
        return currentBoxGuid;
    }
    public bool RemoveIfGuidMatches(Guid guid)
    {
        if (currentBoxGuid == guid)
        {
            SetVisible(false);
            return true;
        }
        return false;
    }

    public void SetVisible(bool visible)
    {
        MainContainer.gameObject.SetActive(visible);
        isVisible = visible;
    }

    public void SetPosition(Vector2 anchoredPosition, Vector2 pivot)
    {
        MainContainer.pivot = pivot;
        MainContainer.anchoredPosition = anchoredPosition;
    }

    public void MarkSelected()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        SetVisible(false);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetVisible(false);
    }
}
