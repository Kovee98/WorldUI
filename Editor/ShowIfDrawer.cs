using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace WorldUI {
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer {
        private MemberInfo cachedMember;
        private string cachedName;
        private object cachedTarget;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            // Skip drawing if height is zero
            if (GetPropertyHeight(property, label) <= 0) {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
            var attr = (ShowIfAttribute)attribute;
            var target = property.serializedObject.targetObject;
            var result = GetMemberValue(target, attr.memberName);

            // If result matches expected, use default height
            if (result != null && result.Equals(attr.expectedValue)) {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // Otherwise, hide
            return 0f;
        }

        private object GetMemberValue (object target, string name) {
            var type = target.GetType();

            // Rebuild cache when target or member changes
            if (cachedMember == null || cachedTarget != target || cachedName != name) {
                cachedTarget = target;
                cachedName = name;
                cachedMember =
                    (MemberInfo)type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
                    (MemberInfo)type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
                    (MemberInfo)type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (cachedMember is FieldInfo field) {
                return field.GetValue(target);
            }

            if (cachedMember is PropertyInfo prop) {
                return prop.GetValue(target, null);
            }

            if (cachedMember is MethodInfo method) {
                return method.Invoke(target, null);
            }

            Debug.LogWarning($"ShowIf: no field, property or method named '{name}' on {type.Name}");

            return null;
        }
    }
}
