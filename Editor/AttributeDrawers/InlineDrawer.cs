using Phoder1.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Phoder1.Editor
{
    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => 0;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is InlineAttribute inlineAttribute && inlineAttribute.UseOriginalLabel)
                property.FlatProperty(label);
            else
                property.FlatProperty();

        }
    }
}