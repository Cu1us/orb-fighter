using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnerStatDisplay : MonoBehaviour
{
    [SerializeField] OrbSpawner Spawner;

    [SerializeField] GameObject SlotsContainer;
    [SerializeField] GameObject StatsContainer;

    [SerializeField] TextMeshProUGUI HealthLabel;
    [SerializeField] TextMeshProUGUI AttackDamageLabel;
    [SerializeField] TextMeshProUGUI SlotsLabel;

    bool showingStats = true;
    void Start()
    {
        UpdateLabels();
        Spawner.onUpgradeAdded += UpdateLabels;
    }
    void UpdateLabels()
    {
        HealthLabel.text = Spawner.MaxHealth.ToString();
        AttackDamageLabel.text = Spawner.AttackDamage.ToString();
        SlotsLabel.text = $"{Spawner.GetUsedSlotsCount()}/{Spawner.MaxSlots}";
    }
    void Update()
    {
        bool doShowStats = ShopItem.HeldShopItem == null || ShopItem.HeldShopItem is not ShopItem_Upgrade;
        if (doShowStats != showingStats)
        {
            showingStats = doShowStats;
            SlotsContainer.SetActive(!showingStats);
            StatsContainer.SetActive(showingStats);
        }
    }
}
