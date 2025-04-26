using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ValueReferenceGeneric<T> : ScriptableObject
{
    [SerializeField] protected T _value;
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            OnValueChanged?.Invoke();
        }
    }
    [SerializeField] private T DefaultValue;

    public Action OnValueChanged;

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
                OnValueChanged = null;
                _value = DefaultValue;
                break;

            case PlayModeStateChange.ExitingEditMode:
                OnValueChanged = null;
                break;
        }
    }
#endif
}
