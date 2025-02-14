using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Auth;
using System;
using System.Data.Common;
using UnityEngine.UIElements;

public class FirebaseManager : MonoBehaviour
{
    static FirebaseDatabase db = null;
    static FirebaseAuth auth = null;
    static FirebaseUser user = null;

    void Awake()
    {
        db = null;
        auth = null;
        user = null;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
                return;
            }

            db = FirebaseDatabase.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db.SetPersistenceEnabled(false);

            db.RootReference.Child("Hello").SetValueAsync("World");
            Debug.Log("Data set!");

            SignIn();
        });
    }

    private void SignIn()
    {
        if (auth == null) Debug.LogWarning("Cannot sign in auth is null.");

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogWarning(task.Exception);
                return;
            }
            user = task.Result.User;
            Debug.Log($"Signed in: user: {user.DisplayName}, id: {user.UserId}");
        });
    }

    public static bool IsDatabaseAvailable()
    {
        return db != null && auth != null && user.IsValid();
    }

    public static void SetData(string path, string data, Action<bool> callback = null)
    {
        if (!IsDatabaseAvailable())
        {
            callback?.Invoke(false);
            Debug.LogWarning($"{nameof(FirebaseManager)}: Couldn't set data at path {path} because the database is unavailable or uninitialized.");
            return;
        }
        db.RootReference.Child(path).SetRawJsonValueAsync(data).ContinueWithOnMainThread(task =>
        {
            bool success = task.IsCompletedSuccessfully;
            callback?.Invoke(success);
            if (!success)
            {
                Debug.LogWarning("1: " + task.Exception);
                Debug.LogWarning("2: " + task.Status);
            }
        });
    }
    public static void LoadData(string path, string data, Action<DataSnapshot> callback = null)
    {
        if (!IsDatabaseAvailable())
        {
            callback?.Invoke(null);
            Debug.LogWarning($"{nameof(FirebaseManager)}: Couldn't load data at path {path} because the database is unavailable or uninitialized.");
            return;
        }
        db.RootReference.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            bool success = task.Exception != null;
            if (!success)
            {
                Debug.LogWarning(task.Exception);
            }
            callback?.Invoke(task.Result);
        });
    }

    public static void LoadRandomEnemyTeam(Action<SerializableTeam?> callback = null)
    {
        if (!IsDatabaseAvailable())
        {
            callback?.Invoke(null);
            Debug.LogWarning($"{nameof(FirebaseManager)}: Couldn't load enemy team because the database is unavailable or uninitialized.");
            return;
        }

    }

    public static void SaveTeam(SerializableTeam teamData, int round)
    {
        string userID = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string serializedTeam = GameSerializer.SerializePlayerTeam(teamData);
        Debug.Log("Trying to save team... User id: " + userID);
        SetData($"teams/random/{round}/{userID}", serializedTeam, success =>
        {
            if (success)
            {
                SetData($"teams/random/{round}/keys/{userID}", "1", success => Debug.Log("Successfully saved team data!"));
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(GameSerializer.SerializePlayerTeam(GameSerializer.GetSerializablePlayerTeam()));
        }
    }
}
