using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeInfoBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Title;

    public Upgrade DisplayedUpgrade;
    public int UpgradeLevel;

    [SerializeField] Color baseColor;
    [SerializeField] Color selectedColor;


    public void OnPointerEnter(PointerEventData eventData)
    {
        Background.color = selectedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Background.color = baseColor;
    }

    public void SetData(Upgrade upgradeToDisplay, int upgradeLevel = 0)
    {
        DisplayedUpgrade = upgradeToDisplay;
        UpgradeLevel = upgradeLevel;
        Icon.sprite = upgradeToDisplay.Icon;
        Title.text = upgradeToDisplay.Name;
        Debug.Log("Data set!");
    }

    public void ClearData()
    {
        Icon.sprite = null;
        Title.text = string.Empty;
        DisplayedUpgrade = null;
        Background.color = baseColor;
    }
}
