using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UpgradeInfoBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Title;

    public Upgrade DisplayedUpgrade;
    public int UpgradeLevel;

    [SerializeField] Color baseColor;
    [SerializeField] Color selectedColor;

    bool selected;
    Guid infoBoxGuid;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }

    void Select()
    {
        selected = true;
        Background.color = selectedColor;
        ShowInfoBox();
    }

    void ShowInfoBox()
    {
        Vector2 infoBoxPosition = Background.rectTransform.position + new Vector3(-Background.rectTransform.rect.width, Background.rectTransform.rect.height) / 2;
        infoBoxGuid = InfoBox.Instance.ShowUpgrade(DisplayedUpgrade, infoBoxPosition);
    }

    void Deselect()
    {
        if (!selected) return;
        selected = false;
        Background.color = baseColor;
        InfoBox.Instance.RemoveIfGuidMatches(infoBoxGuid);
    }

    public void SetData(Upgrade upgradeToDisplay, int upgradeLevel = 0)
    {
        DisplayedUpgrade = upgradeToDisplay;
        UpgradeLevel = upgradeLevel;
        Icon.sprite = upgradeToDisplay.Icon;
        Title.text = upgradeToDisplay.Name;
    }

    public void ClearData()
    {
        Icon.sprite = null;
        Title.text = string.Empty;
        DisplayedUpgrade = null;
        Background.color = baseColor;
    }

}
