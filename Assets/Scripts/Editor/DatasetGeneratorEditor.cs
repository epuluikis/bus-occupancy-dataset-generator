using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatasetGenerator))]
public class DatasetGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            return;
        }

        if (GUILayout.Button("Generate"))
        {
            ((DatasetGenerator)target).Generate();
        }
    }
}
