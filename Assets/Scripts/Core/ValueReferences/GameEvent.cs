using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game Event")]
public class GameEvent : ScriptableObject
{
    public void Invoke()
    {
        //Debug.Log($"Event {name} has been invoked!");
        Event?.Invoke();
    }
    [NonSerialized] public Action Event;

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                Event = null;
                break;

            case PlayModeStateChange.ExitingEditMode:
                Event = null;
                break;
        }
    }
#endif
}
