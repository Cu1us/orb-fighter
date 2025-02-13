using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    FirebaseDatabase db;

    void Start()
    {
        //Setup for talking to firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            db = FirebaseDatabase.DefaultInstance;

            db.RootReference.Child("Hello").SetValueAsync("World");
        });
    }
}
