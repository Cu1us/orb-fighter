using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static ShopItem HeldShopItem;

    [SerializeField] protected RectTransform slot;
    [SerializeField] protected Image image;
    [SerializeField] protected Image ghost;
    [SerializeField] protected TextMeshProUGUI costLabel;
    [SerializeField] protected Image currencyIcon;
    [SerializeField] Color costLabelColor;
    [SerializeField] float TimeBeforeInfoBoxShowsUp;

    public bool IsEmpty;

    protected bool dragging;
    protected Vector2 dragScreenPos;

    bool pointerOver = false;
    float pointerOverStartTime;

    protected Guid infoBoxGuid;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (IsEmpty) return;
        ghost.rectTransform.position = image.rectTransform.position;
        ghost.sprite = image.sprite;
        image.enabled = false;
        ghost.enabled = true;
        dragging = true;
        HeldShopItem = this;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (IsEmpty) return;
        ghost.rectTransform.position += (Vector3)eventData.delta;
        dragScreenPos = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        ResetGhost();
        dragging = false;
        if (HeldShopItem == this)
            HeldShopItem = null;
    }

    void ResetGhost()
    {
        image.enabled = !IsEmpty;
        ghost.enabled = false;
        ghost.rectTransform.position = image.rectTransform.position;
    }

    public virtual int GetCost()
    {
        return 0;
    }

    void OnDisable()
    {
        ResetGhost();
        if (HeldShopItem == this)
            HeldShopItem = null;
    }

    protected virtual void Update()
    {
        if (!IsEmpty)
        {
            if (pointerOver && Time.time - pointerOverStartTime > TimeBeforeInfoBoxShowsUp)
            {
                if (infoBoxGuid == Guid.Empty)
                {
                    ShowInfoBox();
                }
            }
            if (Bank.CanAfford(GetCost()))
            {
                costLabel.color = costLabelColor;
                currencyIcon.color = Color.white;
                ghost.color = Color.white;
            }
            else
            {
                costLabel.color = Color.red;
                currencyIcon.color = Color.red;
                ghost.color = Color.red;
            }
        }
    }

    protected Vector2 GetInfoBoxPosition()
    {
        // NOTE: Not perfect, but I honestly don't know to fix the inaccuracies that occur on non-standard aspect ratios
        return ((Vector2)slot.position + new Vector2(-slot.rect.width, slot.rect.height) / 2) / new Vector2(Screen.width, Screen.height) * GameManager.Instance.MainCanvasScaler.referenceResolution;
    }

    protected virtual void ShowInfoBox()
    {
        // Override in subclasses
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsEmpty) return;
        pointerOver = true;
        pointerOverStartTime = Time.time;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        InfoBox.Instance.RemoveIfGuidMatches(infoBoxGuid);
        infoBoxGuid = Guid.Empty;
    }

    public virtual void SetEmptyState(bool empty)
    {
        IsEmpty = empty;
        if (empty)
        {
            pointerOver = false;
            InfoBox.Instance.RemoveIfGuidMatches(infoBoxGuid);
            infoBoxGuid = Guid.Empty;
            ResetGhost();
            if (HeldShopItem == this)
                HeldShopItem = null;
        }
        image.enabled = !empty;
        ghost.enabled = false;
        costLabel.enabled = !empty;
        currencyIcon.enabled = !empty;
    }
}
