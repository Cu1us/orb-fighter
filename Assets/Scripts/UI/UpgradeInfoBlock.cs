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
        Vector2 infoBoxPosition = Background.rectTransform.position + new Vector3(-Background.rectTransform.rect.width, Background.rectTransform.rect.height) / 2;
        InfoBox.Show(DisplayedUpgrade.Name, DisplayedUpgrade.Description, infoBoxPosition);
    }
    void Deselect()
    {
        if (!selected) return;
        selected = false;
        Background.color = baseColor;
        InfoBox.SetVisible(false);
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
