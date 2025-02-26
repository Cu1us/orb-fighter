using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class OrbSpawner : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [Header("References")]
    [SerializeField] SpriteRenderer highlight;
    [SerializeField] SpriteRenderer overlay;
    [SerializeField] MeshRenderer sphereRenderer;
    [SerializeField][HideInInspector] ArrowRenderer arrowRenderer;
    [SerializeField] Transform VisualEffectsContainer;

    [Header("Settings")]
    public float velocityArrowLength;
    public float holdTimeToMoveObject;

    [Header("Orb data")]
    public Orb Prefab;
    public int MaxSlots;
    public Texture2D Icon;

    public readonly List<Upgrade> Upgrades = new();

    readonly List<OrbVFX> ActiveVFX = new();

    public bool OwnedByPlayer = false;
    public Vector2 StartVelocityDir;
    public float StartVelocityMagnitude;
    public float Mass = 1f;
    public float MaxHealth;
    public float AttackDamage;

    public Action onUpgradeAdded;

    // Local vars
    bool dragging = false;
    bool dragModeMove = false;
    Vector2 dragScreenPosition;
    Vector2 dragStartingPos;
    float highlightUntilTime;

    #region Pointer events
    public void OnBeginDrag(PointerEventData eventData)
    {
        float holdDurationBeforeDrag = Time.unscaledTime - eventData.clickTime;
        dragModeMove = holdDurationBeforeDrag >= holdTimeToMoveObject;
        dragging = true;
        dragScreenPosition = eventData.position;
        dragStartingPos = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragScreenPosition += eventData.delta;
        Vector2 dragWorldPosition = GameManager.Instance.mainCamera.ScreenToWorldPoint(dragScreenPosition);
        if (dragModeMove)
        {
            DragOrbPosition(dragWorldPosition);
        }
        else
        {
            DragStartVelocity(dragWorldPosition);
        }
    }

    void DragOrbPosition(Vector2 dragWorldPosition)
    {
        transform.position = dragWorldPosition;
        float orbRadius = transform.lossyScale.x * 0.5f;
        if (GameManager.CanOrbFitAtPos(dragWorldPosition, orbRadius, gameObject))
        {
            overlay.enabled = false;
        }
        else
        {
            overlay.enabled = true;
            SetOverlayColor(Color.red);
        }
        GameManager.Instance.EnemyAreaWarningBox.enabled = GameManager.IsPointInEnemyArea(dragWorldPosition, orbRadius);
    }

    void DragStartVelocity(Vector2 dragWorldPosition)
    {
        Vector2 velDir = (dragWorldPosition - (Vector2)transform.position) / velocityArrowLength;
        if (velDir.sqrMagnitude > 1)
        {
            velDir = velDir.normalized;
        }
        StartVelocityDir = velDir;
        UpdateVelocityArrow();
    }

    void UpdateVelocityArrow()
    {
        arrowRenderer.DisplayVector(StartVelocityDir * velocityArrowLength);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragModeMove)
        {
            Vector2 dragWorldPosition = GameManager.Instance.mainCamera.ScreenToWorldPoint(dragScreenPosition);
            if (GameManager.CanOrbFitAtPos(dragWorldPosition, transform.lossyScale.x * 0.5f, gameObject))
            {
                transform.position = dragWorldPosition;
            }
            else
            {
                transform.position = dragStartingPos;
            }
            overlay.enabled = false;
            GameManager.Instance.EnemyAreaWarningBox.enabled = false;
        }
        dragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragging) return;
        Debug.Log("Clicked!");
        GameManager.Instance.RegisterClickOnOrbSpawner(this);
    }
    #endregion

    public Orb Spawn(bool disableSelf = true)
    {
        Orb orb = Instantiate(Prefab, transform.position, transform.rotation, GameManager.Instance.SpawnedOrbsContainer);

        orb.OwnedByPlayer = OwnedByPlayer;
        orb.StartVelocity = StartVelocityDir * StartVelocityMagnitude;
        orb.rigidbody.mass = Mass;
        orb.Health = orb.MaxHealth = MaxHealth;
        orb.AttackDamage = AttackDamage;

        orb.renderer.material = sphereRenderer.material;

        AddUpgradesToSpawnedOrb(orb, Upgrades);

        GameManager.Instance.RegisterOrb(orb);

        if (disableSelf) gameObject.SetActive(false);
        return orb;
    }

    private void AddUpgradesToSpawnedOrb(Orb orb, IEnumerable<Upgrade> upgrades)
    {
        Dictionary<OrbBehavior, int> behaviors = new();
        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.AddBehavior && upgrade.BehaviorToAdd)
            {
                if (behaviors.ContainsKey(upgrade.BehaviorToAdd))
                {
                    if (upgrade.UpgradeableBehavior)
                    {
                        behaviors[upgrade.BehaviorToAdd] += upgrade.BehaviorLevel + 1;
                    }
                }
                else
                {
                    behaviors.Add(upgrade.BehaviorToAdd, upgrade.BehaviorLevel);
                }
            }
            if (upgrade.AddStats)
            {
                orb.MaxHealth += upgrade.MaxHealthIncrease;
                orb.AttackDamage += upgrade.AttackDamageIncrease;
                orb.rigidbody.mass += upgrade.MassIncrease;
            }
        }
        foreach (KeyValuePair<OrbBehavior, int> behavior in behaviors)
        {
            orb.AddBehavior(behavior.Key, behavior.Value);
        }
    }
    public int GetUsedSlotsCount()
    {
        int usedSlots = 0;
        foreach (Upgrade upgrade in Upgrades)
            usedSlots += upgrade.SlotsReq;
        return usedSlots;
    }
    public void AddUpgrade(Upgrade upgrade)
    {
        Upgrades.Add(upgrade);
        BehaviorMetadata metadata = upgrade.BehaviorToAdd.Metadata;
        if (metadata && metadata.ApplyVFXToSpawner && metadata.VisualEffectPrefab && !ActiveVFX.Contains(metadata.VisualEffectPrefab))
        {
            OrbVFX vfx = Instantiate(metadata.VisualEffectPrefab, VisualEffectsContainer);
            ActiveVFX.Add(metadata.VisualEffectPrefab);
        }
        onUpgradeAdded?.Invoke();
    }
    public bool CanAddUpgrade(Upgrade upgradeToAdd)
    {
        int existingUpgrades = 0;
        int usedSlots = 0;
        foreach (Upgrade upgrade in Upgrades)
        {
            if (upgrade == upgradeToAdd)
                existingUpgrades++;
            usedSlots += upgradeToAdd.SlotsReq;
        }
        return usedSlots + upgradeToAdd.SlotsReq <= MaxSlots && existingUpgrades < upgradeToAdd.MaxInstancesPerOrb;
    }

    public void Highlight(Color color, float duration = 0)
    {
        highlightUntilTime = Time.time + duration;
        highlight.enabled = true;
        highlight.color = color;
    }
    public void SetOverlayColor(Color color)
    {
        color.a = overlay.color.a;
        overlay.color = color;
    }
    public void SetIcon(Texture2D icon)
    {
        sphereRenderer.material.mainTexture = icon;
    }
    public void SetColor(Color color)
    {
        sphereRenderer.material.color = color;
    }

    void Update()
    {
        if (highlightUntilTime < Time.time)
        {
            highlight.enabled = false;
        }
    }

    public Dictionary<Upgrade, int> GetUpgrades()
    {
        Dictionary<Upgrade, int> upgrades = new();
        foreach (Upgrade upgrade in Upgrades)
        {
            if (upgrades.ContainsKey(upgrade))
            {
                upgrades[upgrade]++;
            }
            else
            {
                upgrades[upgrade] = 0;
            }
        }
        return upgrades;
    }

    public static OrbSpawner InstantiateSpawnerFromData(SerializableOrbSpawner data, Transform parent, bool ownedByPlayer = false)
    {
        OrbSpawner instance = Instantiate(GameManager.Settings.DefaultOrbSpawnerPrefab, data.position, Quaternion.identity, parent);
        instance.StartVelocityDir = data.startVelocity.normalized;
        instance.StartVelocityMagnitude = data.startVelocity.magnitude;
        instance.OwnedByPlayer = ownedByPlayer;
        foreach (string upgradeID in data.upgrades)
        {
            if (GameManager.TryGetUpgradeFromID(upgradeID, out Upgrade upgrade))
            {
                instance.AddUpgrade(upgrade);
            }
            else
            {
                Debug.LogWarning($"Couldn't apply unknown upgrade with ID '{upgrade}' to enemy orb.", instance);
            }
        }
        instance.UpdateIconAndColor();
        return instance;
    }

    void Awake()
    {
        arrowRenderer = GetComponentInChildren<ArrowRenderer>();
        if (arrowRenderer == null)
            Debug.LogError($"Orb spawner \"{name}\": Could not find an arrow renderer among children", this);
        UpdateVelocityArrow();
    }

    void Start()
    {
        UpdateIconAndColor();
    }

    public void UpdateIconAndColor()
    {
        SetIcon(Icon);
        SetColor(OwnedByPlayer ? GameManager.Settings.PlayerOrbColor : GameManager.Settings.EnemyOrbColor);
    }
}