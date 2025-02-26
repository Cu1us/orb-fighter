using UnityEngine;

// You sadly cannot directly create instances of generic SO:s. This workaround allows you to create them anyway by making them non-generic.
[CreateAssetMenu(menuName = "Asset Map/Upgrade Asset Map")]
public class UpgradeAssetMap : AssetMap<Upgrade>
{
}