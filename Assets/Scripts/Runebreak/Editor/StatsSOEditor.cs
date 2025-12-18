using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsSO))]
public class StatsSOEditor : Editor
{
    private SerializedProperty _statsList;
    private SerializedProperty _name;
    private SerializedProperty _prefab;
    private void OnEnable()
    {
        // Link to the private 'stats' list in the SO
        _statsList = serializedObject.FindProperty("_stats");
        _name = serializedObject.FindProperty("Name");
        _prefab = serializedObject.FindProperty("Prefab");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object to get the latest data
        serializedObject.Update();

        // Draw a header (Optional)
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Base Stats Configuration", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(_name, new GUIContent("Name"));
        EditorGUILayout.PropertyField(_prefab, new GUIContent("Prefab"));
        EditorGUILayout.Space();

        if (_statsList != null)
        {
            // Iterate through the list elements
            for (int i = 0; i < _statsList.arraySize; i++)
            {
                SerializedProperty entry = _statsList.GetArrayElementAtIndex(i);
                SerializedProperty statType = entry.FindPropertyRelative("Stat");
                SerializedProperty value = entry.FindPropertyRelative("Value");

                // Get the enum name to use as the label
                // statType.enumDisplayNames gives us the nice string name of the enum
                string label = statType.enumDisplayNames[statType.enumValueIndex];

                // Draw the float field with the Enum name as the label
                // This replaces the "Element X" foldout entirely
                EditorGUILayout.PropertyField(value, new GUIContent(label));
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Stats list not found. Check variable name.", MessageType.Error);
        }

        // Apply changes back to the scriptable object
        serializedObject.ApplyModifiedProperties();
    }
}