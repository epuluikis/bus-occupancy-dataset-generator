using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneGenerator))]
public class SceneGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            return;
        }

        if (GUILayout.Button("Spawn by percentage"))
        {
            ((SceneGenerator)target).SpawnByPercentage();
        }

        if (GUILayout.Button("Spawn by status"))
        {
            ((SceneGenerator)target).SpawnByStatus();
        }

        if (GUILayout.Button("Clear"))
        {
            ((SceneGenerator)target).Clear();
        }
    }
}
