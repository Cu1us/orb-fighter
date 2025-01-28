using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem_Upgrade : ShopItem
{
    [SerializeField] OrbBehavior upgrade;
    [SerializeField] int upgradeLevel;

    bool RaycastForSpawner(out OrbSpawner spawner)
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

    void Update()
    {
        if (dragging)
        {
            if (RaycastForSpawner(out OrbSpawner spawner))
            {
                spawner.Highlight(CanApplyUpgradeTo(spawner) ? Color.green : Color.red);
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (RaycastForSpawner(out OrbSpawner spawner) && CanApplyUpgradeTo(spawner))
        {
            ApplyUpgradeTo(spawner);
        }
    }

    bool CanApplyUpgradeTo(OrbSpawner spawner)
    {
        return true;
    }
    void ApplyUpgradeTo(OrbSpawner spawner)
    {
        spawner.AddBehavior(upgrade);
    }
}
