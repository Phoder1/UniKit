using UnityEditor;

namespace UniKit.Editor
{
    using Editor = UnityEditor.Editor;
    public class SerializedWindow<TChild> : EditorWindow
        where TChild : SingletonScriptableObject<TChild>
    {
        protected SerializedObject serObj;
        protected Editor dataEditor;
        protected TChild Data => SingletonScriptableObject<TChild>.Data;

        protected virtual bool DrawDataInFoldout => false;

        private bool dataFoldout = false;
        protected virtual void LoadData()
        {
            if (serObj == null)
                serObj = new SerializedObject(Data);
            if (dataEditor == null)
                dataEditor = Editor.CreateEditor(Data);
        }

        protected virtual void OnGUI()
        {
            LoadData();

            if (!DrawDataInFoldout
                || (dataFoldout = EditorGUILayout.InspectorTitlebar(dataFoldout, Data)))
                dataEditor.DrawDefaultInspector();
        }
    }
}