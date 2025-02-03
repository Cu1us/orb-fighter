using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] protected Image image;
    [SerializeField] protected Image ghost;
    protected bool dragging;
    protected Vector2 dragScreenPos;

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

    protected bool RaycastForSpawner(out OrbSpawner spawner)
    {
        Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(dragScreenPos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null && hit.transform.TryGetComponent(out spawner))
        {
            return true;
        }
        spawner = null;
        return false;
    }
}
