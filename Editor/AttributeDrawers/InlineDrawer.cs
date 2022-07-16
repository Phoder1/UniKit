using System.Collections.Generic;
using System.Linq;
using UniKit.Attributes;
using UnityEditor;
using UnityEngine;

namespace UniKit.Editor
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineDrawer : PropertyDrawer
    {
        private const float ProperetiesGap = 2f;

        private IEnumerable<SerializedProperty> GetProperties(SerializedProperty property)
        {
            if (!property.hasVisibleChildren)
            {
                yield return property;
                yield break;
            }

            foreach (var child in property.GetChilds())
                yield return child;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => GetProperties(property).Sum((x) => EditorGUI.GetPropertyHeight(x, true) + ProperetiesGap);
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var content = new GUIContent(label);
            bool overrideLabel = attribute is InlineAttribute ia && ia.UseOriginalLabel;
            foreach (var prop in GetProperties(property))
            {
                if (overrideLabel)
                    EditorGUI.PropertyField(position, prop, new GUIContent(content.text, content.image, content.tooltip), true);
                else
                    EditorGUI.PropertyField(position, prop, true);

                float height = EditorGUI.GetPropertyHeight(prop, true)  + ProperetiesGap;
                var pos = position.position;
                pos.y += height;
                position.position = pos;
            }
        }
    }
}
