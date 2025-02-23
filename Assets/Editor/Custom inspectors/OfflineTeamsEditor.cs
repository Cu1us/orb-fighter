using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(OfflineTeamList))]
public class OfflineTeamsEditor : Editor
{
    string inputJSON = "";

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Add new team as JSON:");
        inputJSON = GUILayout.TextArea(inputJSON);
        if (GUILayout.Button("Add to list"))
        {
            bool success = AddTeamFromJSON(inputJSON);
            if (success)
            {
                inputJSON = string.Empty;
            }
        }
        GUILayout.Space(20);

        DrawDefaultInspector();
    }

    bool AddTeamFromJSON(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;

        SerializedProperty serializedList = serializedObject.FindProperty("Teams");
        if (serializedList == null) return false;

        SerializableTeam team = JsonUtility.FromJson<SerializableTeam>(json);
        if (team == null) return false;

        int lastIndex = serializedList.arraySize++;
        SerializedProperty newTeamEntry = serializedList.GetArrayElementAtIndex(lastIndex);
        newTeamEntry.boxedValue = team;

        return serializedObject.ApplyModifiedProperties();
    }
}