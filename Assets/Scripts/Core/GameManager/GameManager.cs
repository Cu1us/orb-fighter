using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Core GameManager behavior. Other modules (partial class implementations) are split into other files in the same folder.
public partial class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance) return _instance;
            else throw new NullReferenceException("A script attempted to access the static GameManager instance, but it has not yet been initialized.");
        }
    }
    public static bool HasInstance { get { return _instance != null; } }

    [Header("Stats")]
    public int RemainingLives;

    [Header("Game Settings Asset")]
    public GameSettings gameSettings;
    public static GameSettings Settings { get { return Instance?.gameSettings; } }


    [Header("General Scene References")]
    public OrbSpawnerContainer PlayerSpawnerContainer;
    public OrbSpawnerContainer EnemySpawnerContainer;
    public Transform SpawnedOrbsContainer;
    public GameObject ShopContainer;

    public SpriteRenderer EnemyAreaWarningBox;
    public CanvasScaler MainCanvasScaler;

    [NonSerialized] public Camera mainCamera;

    [Header("Events")]
    public UnityEvent<OrbSpawner> OnOrbClicked;

    void Start()
    {
        RestartGame();
    }

    public static bool IsPointInEnemyArea(Vector2 point, float margin)
    {
        return point.x >= -margin;
    }

    public static bool CanOrbFitAtPos(Vector2 pos, float orbRadius, GameObject objectToIgnore = null)
    {
        if (IsPointInEnemyArea(pos, orbRadius)) return false;

        List<Collider2D> results = new(4);
        ContactFilter2D filter = new()
        {
            useLayerMask = true,
            layerMask = Instance.gameSettings.LayersThatBlockOrbPlacement
        };
        int count = Physics2D.OverlapCircle(pos, orbRadius, filter, results);
        results.RemoveAll((x) => x.gameObject == objectToIgnore);
        count = results.Count;
        if (count > 0) return false;

        return true;
    }

    public static Upgrade GetUpgradeFromID(string id)
    {
        return Instance.gameSettings.UpgradeMap.GetFromID(id);
    }
    public static bool TryGetUpgradeFromID(string id, out Upgrade upgrade)
    {
        return Instance.gameSettings.UpgradeMap.TryGetFromID(id, out upgrade);
    }
    public static string GetIDFromUpgrade(Upgrade upgrade)
    {
        return Instance.gameSettings.UpgradeMap.GetIDOf(upgrade);
    }
    public static bool TryGetIDFromUpgrade(Upgrade upgrade, out string id)
    {
        return Instance.gameSettings.UpgradeMap.TryGetIDOf(upgrade, out id);
    }

    public void RegisterClickOnOrbSpawner(OrbSpawner source)
    {
        OnOrbClicked?.Invoke(source);
    }


    public void RestartGame()
    {
        Round = 0;
        RemainingLives = Settings.StartingLives;
        Bank.SetBalance(Settings.StartingCurrency);
        PlayerSpawnerContainer.Clear();
        EnemySpawnerContainer.Clear();
    }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        mainCamera = Camera.main;
    }
    void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}