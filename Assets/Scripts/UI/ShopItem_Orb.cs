using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem_Orb : ShopItem
{
    public OrbSpawner spawnerToPlace;
    public LayerMask layersThatBlockPlacement;

    RectTransform shopContainer;

    void Start()
    {
        shopContainer = GameManager.Instance.ShopContainer.GetComponent<RectTransform>();
        ResizeIconsToMatchOrbsInWorld();
    }

    void ResizeIconsToMatchOrbsInWorld()
    {
        Camera mainCamera = Camera.main;
        Vector2 canvasSize = ghost.rectTransform.root.GetComponent<RectTransform>().sizeDelta;

        float imageSize = canvasSize.x / (mainCamera.aspect * 2) / mainCamera.orthographicSize;
        ghost.rectTransform.sizeDelta = imageSize * Vector2.one;

        float slotImageSize = Mathf.Min(imageSize, GetComponent<RectTransform>().sizeDelta.x - 10f);
        image.rectTransform.sizeDelta = slotImageSize * Vector2.one;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        Vector2 point = Camera.main.ScreenToWorldPoint(ghost.rectTransform.position);

        bool inEnemyArea = GameManager.IsPointInEnemyArea(point, spawnerToPlace.transform.lossyScale.x * 0.5f);
        bool isAboveShopUI = IsAboveShopUI();

        GameManager.Instance.EnemyAreaWarningBox.enabled = inEnemyArea && !isAboveShopUI;

        (int overlappingCount, List<Collider2D> overlappingColliders) = GetOverlappingColliders(point);

        if ((overlappingCount > 0 || inEnemyArea) && !isAboveShopUI)
        {
            ghost.color = Color.red;
        }
        else
        {
            ghost.color = Color.white;
        }
    }

    bool IsAboveShopUI()
    {
        Vector2 point = ghost.rectTransform.position;
        // a recttransform's width annoyingly deviates from its actual pixel size on the screen if screen resolution != canvas scaler reference resolution
        return point.x > Screen.width - shopContainer.rect.width / GameManager.Instance.MainCanvasScaler.referenceResolution.x * Screen.width;

    }

    (int, List<Collider2D>) GetOverlappingColliders(Vector2 point)
    {
        List<Collider2D> results = new(4);
        ContactFilter2D filter = new()
        {
            useLayerMask = true,
            layerMask = layersThatBlockPlacement
        };
        int count = Physics2D.OverlapCircle(point, spawnerToPlace.transform.lossyScale.x * 0.5f, filter, results);
        return (count, results);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        GameManager.Instance.EnemyAreaWarningBox.enabled = false;

        Vector2 point = Camera.main.ScreenToWorldPoint(ghost.rectTransform.position);

        if (!GameManager.IsPointInEnemyArea(point, spawnerToPlace.transform.lossyScale.x * 0.5f) && !IsAboveShopUI())
        {
            (int overlappingCount, _) = GetOverlappingColliders(point);
            if (overlappingCount == 0)
            {
                PlaceSpawnerAt(point);
            }
        }

        base.OnEndDrag(eventData);
    }

    void PlaceSpawnerAt(Vector3 spawnPoint)
    {
        OrbSpawner spawner = Instantiate(spawnerToPlace, spawnPoint, Quaternion.identity, GameManager.Instance.PlayerSpawnerContainer.transform);
        GameManager.Instance.PlayerSpawnerContainer.AddSpawner(spawner);
    }
}
