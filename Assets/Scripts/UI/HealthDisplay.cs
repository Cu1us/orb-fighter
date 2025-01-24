using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] Orb orbToListenTo;
    [SerializeField] TextMeshProUGUI textToSet;

    void Awake()
    {
        orbToListenTo.onSpawn += UpdateDisplay;
        orbToListenTo.onTakeDamage += (_) => UpdateDisplay();
    }

    void UpdateDisplay()
    {
        textToSet.text = $"HP: {orbToListenTo.Health}/{orbToListenTo.MaxHealth}";
    }
}
