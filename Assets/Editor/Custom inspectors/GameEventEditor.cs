
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(GameEvent))]
public class GameEvent_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        GameEvent gameEvent = (GameEvent)target;
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Fire Event"))
            {
                gameEvent?.Invoke();
            }
        }
        else
        {
            GUILayout.Label("Enter Play Mode to fire manually.");
        }
    }
}