using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using TMPro;

// GameManager round management logic.
public partial class GameManager
{
    public static State GameState { get; private set; } = State.SHOP;
    public static int Round = 0;

    public Action onTakeoffAllOrbs;

    readonly List<Orb> ActivePlayerOrbs = new();
    readonly List<Orb> ActiveEnemyOrbs = new();

    public UnityEvent OnRoundStart;
    public UnityEvent<RoundResult> OnRoundEnd;
    public UnityEvent OnEnterShop;

    public bool gameActive;

    public TextMeshProUGUI RoundEndText;



    public void RegisterOrb(Orb orb)
    {
        if (orb.OwnedByPlayer)
        {
            ActivePlayerOrbs.Add(orb);
        }
        else
        {
            ActiveEnemyOrbs.Add(orb);
        }
    }

    public void UnregisterOrb(Orb orb)
    {
        ActivePlayerOrbs.Remove(orb);
        ActiveEnemyOrbs.Remove(orb);
        CheckForRoundEnd();
    }

    void CheckForRoundEnd()
    {
        if (!gameActive || (ActivePlayerOrbs.Count > 0 && ActiveEnemyOrbs.Count > 0))
            return; // Game still ongoing

        if (ActivePlayerOrbs.Count == 0)
        {
            RoundEnd(RoundResult.LOSS);
        }
        else if (ActiveEnemyOrbs.Count == 0)
        {
            RoundEnd(RoundResult.VICTORY);
        }
        else
        {
            RoundEnd(RoundResult.DRAW);
        }
    }

    void ClearAllActiveOrbs()
    {
        foreach (Transform child in SpawnedOrbsContainer)
        {
            Destroy(child.gameObject);
        }
        ActivePlayerOrbs.Clear();
        ActiveEnemyOrbs.Clear();
    }

    void SetShopActive(bool active)
    {
        ShopContainer.SetActive(active);
    }

    public IEnumerable GetAllOrbs()
    {
        foreach (Orb orb in ActivePlayerOrbs)
        {
            yield return orb;
        }
        foreach (Orb orb in ActiveEnemyOrbs)
        {
            yield return orb;
        }
    }


    [ContextMenu("Start Next Round")]
    public void LoadAndStartNextRound()
    {
        GameState = State.FIND_OPPONENT;
        Round++;
        GetEnemyTeam(team =>
        {
            EnemySpawnerContainer.SetupTeam(team);
            StartRound();
        });
    }
    void GetEnemyTeam(Action<SerializableTeam> callback = null)
    {
        FirebaseManager.TryLoadRandomEnemyTeam(Round, team =>
        {
            if (team == null)
            {
                Debug.Log("Team to set up was null. Offline teams not implemented yet.");
                // TODO: Get offline team
            }
            else
            {
                callback?.Invoke(team);
            }
        });
    }

    public void StartRound()
    {
        gameActive = true;
        GameState = State.COMBAT;
        OnRoundStart?.Invoke();

        SetShopActive(false);

        PlayerSpawnerContainer.SpawnAll();
        PlayerSpawnerContainer.Hide();

        EnemySpawnerContainer.SpawnAll();
        EnemySpawnerContainer.Hide();

        Invoke(nameof(TakeoffAllOrbs), 1f);
    }

    void TakeoffAllOrbs()
    {
        onTakeoffAllOrbs?.Invoke();
    }

    public void TryStartNextRound()
    {
        if (GameState == State.SHOP)
        {
            LoadAndStartNextRound();
        }
    }

    public void RoundEnd(RoundResult result)
    {
        gameActive = false;
        OnRoundEnd?.Invoke(result);
        Invoke(nameof(BackToShop), 3);

        RoundEndText.text = result switch
        {
            RoundResult.VICTORY => "YOU WIN !!!",
            RoundResult.LOSS => "YOU LOSE !!! >:(",
            _ => "DRAW??!?!?!"
        };

        FirebaseManager.SaveTeam(GameSerializer.GetSerializablePlayerTeam(), Round);

        RoundEndText.gameObject.SetActive(true);
    }

    public void BackToShop()
    {
        CancelInvoke(nameof(BackToShop));

        RoundEndText.gameObject.SetActive(false);

        OnEnterShop?.Invoke();
        ClearAllActiveOrbs();
        GameState = State.SHOP;
        SetShopActive(true);
        PlayerSpawnerContainer.Show();
        EnemySpawnerContainer.Clear();
    }


    public enum State
    {
        SHOP,
        FIND_OPPONENT,
        COMBAT
    }
    public enum RoundResult
    {
        VICTORY,
        LOSS,
        DRAW
    }
}
