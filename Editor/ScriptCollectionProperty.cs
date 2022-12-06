using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UniKit.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;
    [CustomPropertyDrawer(typeof(ScriptCollection<>))]
    public class ScriptCollectionProperty : PropertyDrawer
    {
        private static readonly string RootPath = Path.Combine("Assets", "ScriptCollections");
        private Type _scriptCollectionType;
        private Type ScriptCollectionType
        {
            get
            {
                if (_scriptCollectionType == null)
                    _scriptCollectionType = fieldInfo.FieldType.GenericTypeArguments[0];

                return _scriptCollectionType;
            }
        }

        private MethodInfo _createInstanceMethod;
        private MethodInfo CreateInstanceMethod
        {
            get
            {
                if (_createInstanceMethod == null)
                    _createInstanceMethod = typeof(ScriptableObject).GetMethods().First(m => m.Name == "CreateInstance" && m.GetParameters().Any(p => p.ParameterType == typeof(Type)));

                return _createInstanceMethod;
            }
        }


        private UnityEditorInternal.ReorderableList _reorderableList;
        private UnityEditorInternal.ReorderableList ReorderableList
        {
            get
            {
                if (_reorderableList == null)
                    _reorderableList = InitList();

                return _reorderableList;
            }
        }

        private ReorderableList InitList()
        {
            var reorderableList = new UnityEditorInternal.ReorderableList(serializedObject: scriptsProperty.serializedObject, elements: scriptsProperty, true, true, true, true);

            return reorderableList;
        }


        private GenericMenu addButtonOptions;
        private SerializedProperty scriptsProperty;
        private GUIContent AddButtonContent = new GUIContent("Add Script");
        private Dictionary<Object, Editor> _scriptEditors = new Dictionary<Object, Editor>();
        private float[] _heightArray;
        private bool focusedFoldout = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitGUI();
            InitList();

            ReorderableList.DoLayoutList();

            DrawFocusedObject();

            void InitGUI()
            {
                if (scriptsProperty == null)
                    scriptsProperty = property.FindPropertyRelative("_scripts");

                if (addButtonOptions == null)
                    addButtonOptions = GetAddScriptMenu(property);

                int scriptsArraySize = scriptsProperty.arraySize;

                RemoveAllNullScripts();

                if (_heightArray == null)
                    _heightArray = new float[scriptsArraySize];
                else if (_heightArray.Length != scriptsArraySize)
                    Array.Resize(ref _heightArray, scriptsArraySize);
            }

            void InitList()
            {
                ReorderableList.drawHeaderCallback = DrawHeader;
                ReorderableList.drawElementCallback = DrawElement;
                ReorderableList.onAddDropdownCallback = AddDropdown;
                ReorderableList.onRemoveCallback = Remove;
                ReorderableList.multiSelect = true;
            }

            void DrawFocusedObject()
            {
                int selected = 0;
                SerializedProperty focusedScript = null;
                for (int i = 0; i < ReorderableList.count; i++)
                {
                    if (ReorderableList.IsSelected(i))
                    {
                        selected++;

                        if (selected > 1)
                            return;

                        focusedScript = scriptsProperty.GetArrayElementAtIndex(i);
                    }
                }

                if (selected == 0)
                    return;

                if (focusedScript != null)
                {
                    var focusedObj = focusedScript.objectReferenceValue;

                    focusedFoldout = EditorGUILayout.InspectorTitlebar(focusedFoldout, focusedScript.objectReferenceValue);

                    if (focusedFoldout)
                        Draw(focusedObj);
                }
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var element = scriptsProperty.GetArrayElementAtIndex(index);
                var objRef = element.objectReferenceValue;
                EditorGUI.LabelField(rect, $"{objRef.GetType()} ({objRef.name})");
            }

            void AddDropdown(Rect buttonRect, ReorderableList list)
            {
                addButtonOptions.ShowAsContext();
            }


            void DrawHeader(Rect rect)
            {
                EditorGUI.LabelField(rect, label);
            }

            void Remove(ReorderableList list)
            {
                for (int i = list.count - 1; i >= 0; i--)
                {
                    if (list.IsSelected(i))
                    {
                        RemoveElementAtIndex(i);
                        list.Deselect(i);
                    }
                }

                void RemoveElementAtIndex(int index)
                {
                    var element = scriptsProperty.GetArrayElementAtIndex(index);
                    var objRef = element.objectReferenceValue;

                    if (_scriptEditors.TryGetValue(objRef, out var editor))
                    {
                        Object.DestroyImmediate(editor);
                        _scriptEditors.Remove(objRef);
                    }

                    scriptsProperty.DeleteArrayElementAtIndex(index);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(objRef));
                    Object.DestroyImmediate(objRef);
                }
            }
        }


        private void Draw(Object focusedObj)
        {
            var editor = _scriptEditors.GetOrInsert(focusedObj, focusedObj.CreateEditor);
            focusedObj.CreateCachedEditor(ref editor);
            editor.OnInspectorGUI();
        }

        private void RemoveAllNullScripts()
        {
            for (int i = 0; i < scriptsProperty.arraySize; i++)
            {
                var script = scriptsProperty.GetArrayElementAtIndex(i);

                if (script.objectReferenceValue == null)
                    scriptsProperty.DeleteArrayElementAtIndex(i);
            }
        }

        private GenericMenu GetAddScriptMenu(SerializedProperty property)
        {
            var allTypes = ScriptCollectionType.GetAllThatImplement(true, true).ToArray();
            var implementingTypes = allTypes.Where(t => !t.IsAbstract);
            var menu = new GenericMenu();
            foreach (var type in implementingTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, AddType);

                void AddType()
                {
                    Debug.Log("Adding type " + type.Name);
                    var script = CreateScriptInstance(property, type);
                    scriptsProperty.arraySize++;
                    var element = scriptsProperty.GetArrayElementAtIndex(scriptsProperty.arraySize - 1);
                    element.objectReferenceValue = script;
                    scriptsProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            return menu;
        }

        private ScriptableObject CreateScriptInstance(SerializedProperty property, Type type)
        {
            var instance = (ScriptableObject)CreateInstanceMethod.Invoke(null, new object[] { type });
            var path = GenerateNewPath(property, instance);
            CreateFolderForPath(path);
            AssetDatabase.CreateAsset(instance, path);
            return instance;
        }

        private void CreateFolderForPath(string path)
        {
            path = path.Replace('\\', '/');
            var directoryNames = path.Split("/");
            string current = string.Empty;
            for (int i = 0; i < directoryNames.Length - 1; i++)
            {
                string directory = directoryNames[i];

                var next = Path.Combine(current, directory);

                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, directory);

                current = next;
            }
        }

        private string GenerateNewPath(SerializedProperty property, ScriptableObject scriptableObject)
        {
            var path = Path.Combine(RootPath, property.serializedObject.targetObject.name, property.name, ScriptCollectionType.Name, $"{scriptableObject.GetType().Name}_{scriptableObject.GetInstanceID()}");
            return Path.ChangeExtension(path, "asset");
        }
    }
}
