using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniKit.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    [InitializeOnLoad]
    public class BookmarkLabelEditor : Editor
    {
        static BookmarkLabelEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemGui;
        }

        private static void HierarchyItemGui(int instanceID, Rect selectionRect)
        {
            string text = "ID: " + instanceID.ToString();

            if (AssetDatabase.IsMainAsset(instanceID))
                text = "(main)";

            var content = new GUIContent(text);

            EditorGUI.LabelField(GetClampedRight(selectionRect, EditorStyles.largeLabel.CalcSize(content).x), content, EditorStyles.largeLabel);
            //EditorGUI.DrawRect(selectionRect, Color.red);
        }

        public static Rect GetRightSection(Rect origin, float weight)
        {
            float newSize = origin.width * weight;
            float offset = (newSize - origin.width) / 2;


            return new Rect(origin.position - Vector2.right * offset, new Vector2(newSize, origin.height));
        }
        public static Rect GetClampedRight(Rect origin, float newWidth)
        {
            float offset = origin.width - newWidth;


            return new Rect(origin.position + Vector2.right * offset, new Vector2(newWidth, origin.height));
        }
    }
}
