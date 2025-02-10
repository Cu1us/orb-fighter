using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected RectTransform slot;
    [SerializeField] protected Image image;
    [SerializeField] protected Image ghost;
    [SerializeField] float TimeBeforeInfoBoxShowsUp;

    protected bool dragging;
    protected Vector2 dragScreenPos;

    bool pointerOver = false;
    float pointerOverStartTime;

    protected Guid infoBoxGuid;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        ghost.rectTransform.position = image.rectTransform.position;
        ghost.sprite = image.sprite;
        image.enabled = false;
        ghost.enabled = true;
        dragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        ghost.rectTransform.position += (Vector3)eventData.delta;
        dragScreenPos = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        ResetGhost();
        dragging = false;
    }

    void ResetGhost()
    {
        image.enabled = true;
        ghost.enabled = false;
        ghost.rectTransform.position = image.rectTransform.position;
    }

    void OnDisable()
    {
        ResetGhost();
    }

    protected virtual void Update()
    {
        if (pointerOver && Time.time - pointerOverStartTime > TimeBeforeInfoBoxShowsUp)
        {
            if (infoBoxGuid == Guid.Empty)
            {
                ShowInfoBox();
            }
        }
    }

    protected Vector2 GetInfoBoxPosition()
    {
        return slot.position + new Vector3(-slot.rect.width, slot.rect.height) / 2;
    }

    protected virtual void ShowInfoBox()
    {
        // Override in subclasses
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
        pointerOverStartTime = Time.time;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        InfoBox.Instance.RemoveIfGuidMatches(infoBoxGuid);
        infoBoxGuid = Guid.Empty;
    }
}
