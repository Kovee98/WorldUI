using UnityEditor;
using UnityEngine;

namespace WorldUI {
    [CustomEditor(typeof(WorldUILayout), true)]
    public class WorldUILayoutEditor : Editor {
        public override void OnInspectorGUI () {
            // Get the target object and call the updater.
            WorldUILayout layout = (WorldUILayout)target;
            WorldUIUpdater updater = layout.GetWorldUIUpdater();

            serializedObject.Update();

            // Check for any changes to properties within this block.
            EditorGUI.BeginChangeCheck();

            // Draw all serialized properties of the WorldUILayout.
            DrawPropertiesExcluding(serializedObject, "m_Script");

            if (GUILayout.Button("Update Now")) {
                if (updater != null) updater.UpdateUI();
            }

            if (EditorGUI.EndChangeCheck()) {
                // Apply the property changes.
                serializedObject.ApplyModifiedProperties();
                if (updater != null) updater.UpdateUIThrottled();
            }
        }
    }
}
