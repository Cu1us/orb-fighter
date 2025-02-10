using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ShopItem_Upgrade : ShopItem
{
    public Upgrade upgrade;

    protected override void Update()
    {
        if (dragging)
        {
            if (RaycastForSpawner(out OrbSpawner spawner))
            {
                spawner.Highlight(CanApplyUpgradeTo(spawner) ? Color.green : Color.red);
            }
        }
        base.Update();
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
        return upgrade.CanApplyTo(spawner);
    }

    void ApplyUpgradeTo(OrbSpawner spawner)
    {
        upgrade.ApplyToSpawner(spawner);
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

    protected override void ShowInfoBox()
    {
        infoBoxGuid = InfoBox.Instance.ShowUpgrade(upgrade, GetInfoBoxPosition());
    }
}
