using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomManager), true)]
public class RandomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
        {
            return;
        }

        if (GUILayout.Button("Randomize"))
        {
            ((RandomManager)target).Randomize();
        }
    }
}
