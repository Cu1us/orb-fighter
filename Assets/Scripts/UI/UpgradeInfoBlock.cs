using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeInfoBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Icon;
    public TextMeshProUGUI Title;

    public Upgrade DisplayedUpgrade;

    [SerializeField] Color baseColor;
    [SerializeField] Color selectedColor;


    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
