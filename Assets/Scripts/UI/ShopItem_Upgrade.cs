using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopItem_Upgrade : ShopItem
{
    public Upgrade upgrade;

    [SerializeField] TextMeshProUGUI slotsRequiredLabel;
    [SerializeField] Image slotsRequiredIcon;

    public void SetUpgrade(Upgrade upgrade)
    {
        this.upgrade = upgrade;
        UpdateData();
    }
    void UpdateData()
    {
        SetEmptyState(false);
        image.sprite = upgrade.Icon;
        ghost.sprite = upgrade.Icon;
        slotsRequiredLabel.text = upgrade.SlotsReq.ToString();
        costLabel.text = GetCost().ToString();
    }

    void Start()
    {
        UpdateData();
    }

    public override void SetEmptyState(bool empty)
    {
        base.SetEmptyState(empty);
        slotsRequiredLabel.enabled = !empty;
        slotsRequiredIcon.enabled = !empty;
    }

    protected override void Update()
    {
        if (dragging && !IsEmpty)
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
        if (!IsEmpty && RaycastForSpawner(out OrbSpawner spawner) && CanApplyUpgradeTo(spawner))
        {
            if (Bank.TryDeduct(GetCost()))
            {
                ApplyUpgradeTo(spawner);
                SetEmptyState(true);
            }
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

    public override int GetCost()
    {
        return upgrade.Cost;
    }

    protected override void ShowInfoBox()
    {
        infoBoxGuid = InfoBox.Instance.ShowUpgrade(upgrade, GetInfoBoxPosition());
    }
}
