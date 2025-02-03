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
    [SerializeField] Transform VisualEffectsContainer;
    [SerializeField][HideInInspector] SpriteRenderer spriteRenderer;
    [SerializeField][HideInInspector] ArrowRenderer arrowRenderer;

    [Header("Settings")]
    public float velocityArrowLength;
    public float holdTimeToMoveObject;

    [Header("Orb data")]
    public Orb Prefab;

    public Dictionary<OrbBehavior, BehaviorOptions> Behaviors = new();

    readonly List<OrbVFX> ActiveVFX = new();

    public bool OwnedByPlayer = false;
    public Vector2 StartVelocityDir;
    public float StartVelocityMagnitude;
    public float Mass = 1f;
    public float MaxHealth;
    public float AttackDamage;

    // Local vars
    bool dragging = false;
    bool dragModeMove = false;
    Vector2 dragScreenPosition;
    Vector2 dragStartingPos;
    int baseOrderInLayer;
    float highlightUntilTime;

    #region Pointer events
    public void OnBeginDrag(PointerEventData eventData)
    {
        float holdDurationBeforeDrag = Time.unscaledTime - eventData.clickTime;
        dragModeMove = holdDurationBeforeDrag >= holdTimeToMoveObject;
        dragging = true;
        dragScreenPosition = eventData.position;
        dragStartingPos = transform.position;
        baseOrderInLayer = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = baseOrderInLayer + 1;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragScreenPosition += eventData.delta;
        Vector2 dragWorldPosition = GameManager.Instance.mainCamera.ScreenToWorldPoint(dragScreenPosition);
        Debug.DrawLine(transform.position, dragWorldPosition, Color.red, 0.2f);
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
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.red;
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
            spriteRenderer.color = Color.white;
            GameManager.Instance.EnemyAreaWarningBox.enabled = false;
        }
        spriteRenderer.sortingOrder = baseOrderInLayer;
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

        if (!OwnedByPlayer)
        {
            orb.SetColor(Color.red);
        }

        foreach (KeyValuePair<OrbBehavior, BehaviorOptions> behavior in Behaviors)
        {
            orb.AddBehavior(behavior.Key, behavior.Value);
        }

        GameManager.Instance.RegisterOrb(orb);

        if (disableSelf) gameObject.SetActive(false);
        return orb;
    }

    public void AddBehavior(OrbBehavior behavior)
    {
        BehaviorOptions parameters = new(0);
        AddBehavior(behavior, parameters);
    }
    public void AddBehavior(OrbBehavior behavior, BehaviorOptions parameters)
    {
        if (Behaviors.ContainsKey(behavior))
        {
            BehaviorOptions newParameters = Behaviors[behavior];
            if (newParameters.level == parameters.level)
            {
                newParameters.level++;
            }
            else if (parameters.level > newParameters.level)
            {
                newParameters.level = parameters.level;
            }
            Behaviors[behavior] = newParameters;
        }
        else
        {
            Behaviors.Add(behavior, parameters);
            if (behavior.Metadata && behavior.Metadata.ApplyVFXToSpawner && behavior.Metadata.VisualEffectPrefab)
            {
                OrbVFX vfx = Instantiate(behavior.Metadata.VisualEffectPrefab, VisualEffectsContainer);
                ActiveVFX.Add(vfx);
            }
        }
    }

    public void Highlight(Color color, float duration = 0)
    {
        highlightUntilTime = Time.time + duration;
        highlight.enabled = true;
        highlight.color = color;
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        if (upgrade.AddStats)
        {
            MaxHealth += upgrade.MaxHealthIncrease;
            AttackDamage += upgrade.AttackDamageIncrease;
            Mass += upgrade.MassIncrease;
        }
        if (upgrade.AddBehavior && upgrade.BehaviorToAdd)
        {
            BehaviorOptions parameters = new(upgrade.BehaviorLevel);
            AddBehavior(upgrade.BehaviorToAdd, parameters);
        }
    }

    void Update()
    {
        if (highlightUntilTime < Time.time)
        {
            highlight.enabled = false;
        }
    }

    void Awake()
    {
        arrowRenderer = GetComponentInChildren<ArrowRenderer>();
        if (arrowRenderer == null)
            Debug.LogError($"Orb spawner \"{name}\": Could not find an arrow renderer among children", this);
        UpdateVelocityArrow();
    }

    [ContextMenu("Reset private component references")]
    void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}