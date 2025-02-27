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

    public UnityEvent OnFindTeam;
    public UnityEvent<SerializableTeam> OnFoundTeam;
    public UnityEvent<int> OnRoundStart;
    public UnityEvent<RoundResult> OnRoundEnd;
    public UnityEvent OnEnterShop;

    public bool GameActive;
    public float GameStartTime;
    public float GameTime => Time.time - GameStartTime;

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
        if (!GameActive || (ActivePlayerOrbs.Count > 0 && ActiveEnemyOrbs.Count > 0))
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
        OnFindTeam?.Invoke();
        GetEnemyTeam(team =>
        {
            EnemySpawnerContainer.SetupTeam(team);
            OnFoundTeam?.Invoke(team);
            StartRound();
        });
    }
    void GetEnemyTeam(Action<SerializableTeam> callback = null)
    {
        FirebaseManager.TryLoadRandomEnemyTeam(Round, team =>
        {
            if (team == null)
            {
                Debug.LogWarning($"Received team for round {Round} was null. Getting offline team.");
                team = Settings.GetOfflineTeam(Round);
                if (team != null)
                {
                    callback.Invoke(team);
                }
                else
                {
                    Debug.LogError("Failed to get offline team. Returning empty team.");
                    callback.Invoke(new SerializableTeam(new SerializableOrbSpawner[0]));
                }
            }
            else
            {
                callback?.Invoke(team);
            }
        });
    }

    public void StartRound()
    {
        GameActive = true;
        GameState = State.COMBAT;
        OnRoundStart?.Invoke(Round);

        PlayerSpawnerContainer.SpawnAll();
        PlayerSpawnerContainer.Hide();

        EnemySpawnerContainer.SpawnAll();
        EnemySpawnerContainer.Hide();

        GameStartTime = Time.time - 1;
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
        GameActive = false;
        OnRoundEnd?.Invoke(result);

        Bank.Add(Settings.StartingCurrency);

        RoundEndText.text = result switch
        {
            RoundResult.VICTORY => "YOU WIN !!!",
            RoundResult.LOSS => "YOU LOSE :(",
            _ => "DRAW !!!"
        };

        switch (result)
        {
            case RoundResult.VICTORY:
                RoundEndText.text = "YOU WIN !!!";
                goto default;

            case RoundResult.DRAW:
                RoundEndText.text = "DRAW !!!";
                goto default;

            case RoundResult.LOSS:
                RemainingLives--;
                if (RemainingLives < 0)
                {
                    RestartGame();
                    RoundEndText.text = "GAME OVER !!!";
                    Invoke(nameof(BackToShop), 5);
                }
                else
                {
                    RoundEndText.text = $"YOU LOSE !!!\n{RemainingLives} lives remaining";
                    goto default;
                }
                break;

            default:
                Invoke(nameof(BackToShop), 3);
                FirebaseManager.SaveTeam(GameSerializer.GetSerializablePlayerTeam(), Round);
                break;
        }


        RoundEndText.gameObject.SetActive(true);
    }

    public void BackToShop()
    {
        CancelInvoke(nameof(BackToShop));

        RoundEndText.gameObject.SetActive(false);

        GameState = State.SHOP;
        OnEnterShop?.Invoke();
        ClearAllActiveOrbs();
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
