using UnityEngine;

// You sadly cannot directly create instances of generic SO:s. This workaround allows you to create them anyway by making them non-generic.
[CreateAssetMenu(menuName = "Weight Table/Upgrade")]
public class UpgradeWeightTable : WeightTable<Upgrade>
{
}