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

    protected override (string, string) GetInfoBoxData()
    {
        string description = upgrade.Description;
        if (upgrade.AddStats)
        {
            description += "\n\nWhen applied on orb:";
            if (upgrade.MaxHealthIncrease != 0) description += $"\n{(upgrade.MaxHealthIncrease >= 0 ? "+" + upgrade.MaxHealthIncrease : upgrade.MaxHealthIncrease)} Health";
            if (upgrade.AttackDamageIncrease != 0) description += $"\n{(upgrade.AttackDamageIncrease >= 0 ? "+" + upgrade.AttackDamageIncrease : upgrade.AttackDamageIncrease)} Attack damage";
        }
        return (upgrade.Name, description);
    }
}
