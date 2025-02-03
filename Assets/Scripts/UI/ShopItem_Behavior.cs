using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem_Behavior : ShopItem
{
    [SerializeField] OrbBehavior behaviorToAdd;
    [SerializeField] int upgradeLevel;

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

    protected virtual bool CanApplyUpgradeTo(OrbSpawner spawner)
    {
        if (spawner.Behaviors.ContainsKey(behaviorToAdd))
        {
            int existingLevel = spawner.Behaviors[behaviorToAdd].level;
            if (existingLevel > upgradeLevel || existingLevel == behaviorToAdd.Metadata.MaxLevel)
            {
                return false;
            }
        }
        return true;
    }

    protected virtual void ApplyUpgradeTo(OrbSpawner spawner)
    {
        BehaviorOptions behaviorOptions = new(upgradeLevel);
        spawner.AddBehavior(behaviorToAdd, behaviorOptions);
    }
}
