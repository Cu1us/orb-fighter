using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
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
        textToSet.text = $"ATK: {orbToListenTo.AttackDamage}";
    }
}
