using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGrid : MonoBehaviour
{
    [SerializeField] ShopItem_Orb[] OrbSlots;
    [SerializeField] ShopItem_Upgrade[] UpgradeSlots;
    [SerializeField] int MaxActiveSlots;

    public void OnEnterShop()
    {
        RefreshShop();
    }

    void Start()
    {
        RefreshShop();
    }

    public void RefreshShop()
    {
        int orbs = Random.Range(2,4);
        int upgrades = Random.Range(2,3);
        Fill(orbs, upgrades);
    }

    public void Fill(int orbs, int upgrades)
    {
        orbs = Mathf.Clamp(orbs, 0, MaxActiveSlots);
        upgrades = Mathf.Clamp(upgrades, 0, MaxActiveSlots - orbs);

        for (int i = 0; i < OrbSlots.Length; i++)
        {
            bool active = i < orbs;
            OrbSlots[i].gameObject.SetActive(active);
            OrbSlots[i].SetEmptyState(!active);
            if (active)
            {
                OrbSlots[i].SetOrbType(GetRandomOrbType());
            }
        }
        for (int i = 0; i < UpgradeSlots.Length; i++)
        {
            bool active = i < upgrades;
            UpgradeSlots[i].gameObject.SetActive(active);
            UpgradeSlots[i].SetEmptyState(!active);
            if (active)
            {
                UpgradeSlots[i].SetUpgrade(GetRandomUpgrade());
            }
        }
    }

    public Upgrade GetRandomUpgrade()
    {
        return GameManager.Settings.ShopUpgradeWeightTable.PickRandom();
    }
    public OrbType GetRandomOrbType()
    {
        return GameManager.Settings.ShopOrbTypeWeightTable.PickRandom();
    }
}
