using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] protected Image image;
    [SerializeField] protected Image ghost;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        ghost.rectTransform.position = image.rectTransform.position;
        ghost.sprite = image.sprite;
        image.enabled = false;
        ghost.enabled = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        ghost.rectTransform.position += (Vector3)eventData.delta;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        ResetGhost();
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
}
