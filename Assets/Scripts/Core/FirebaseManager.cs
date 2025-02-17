using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Auth;
using Random = UnityEngine.Random;

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
    public static void LoadData(string path, Action<DataSnapshot> callback = null)
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

    public static async void TryLoadRandomEnemyTeam(int round, Action<SerializableTeam> callback = null)
    {
        if (!IsDatabaseAvailable())
        {
            callback?.Invoke(null);
            Debug.LogWarning($"{nameof(FirebaseManager)}: Couldn't load enemy team because the database is unavailable or uninitialized.");
            return;
        }

        // Get list of keys
        Task<DataSnapshot> task = db.RootReference.Child($"teams/random/{round}/keys").GetValueAsync();
        DataSnapshot data = await task;
        if (!task.IsCompletedSuccessfully)
        {
            Debug.LogWarning($"Failed to load team keys for round {round}: {task.Exception}");
            callback?.Invoke(null);
            return;
        }
        string json = data.GetRawJsonValue();
        Debug.Log("Loaded JSON for key list: " + json);

        // Pick random key
        List<string> validKeyList = GameSerializer.CustomParseJSONTeamKeyList(json);
        string randomKey = validKeyList[Random.Range(0, validKeyList.Count)];
        Debug.Log("Loaded key: " + randomKey);

        // Get team at key
        task = db.RootReference.Child($"teams/random/{round}/{randomKey}").GetValueAsync();
        data = await task;
        if (!task.IsCompletedSuccessfully)
        {
            Debug.LogWarning($"Failed to load team at randomly selected key '{randomKey}' for round {round}: {task.Exception}");
            callback?.Invoke(null);
            return;
        }
        json = data.GetRawJsonValue();

        SerializableTeam team = JsonUtility.FromJson<SerializableTeam>(json);
        Debug.Log("Team loaded successfully!");
        callback?.Invoke(team);
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
