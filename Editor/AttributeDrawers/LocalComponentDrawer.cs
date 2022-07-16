using System;
using UnityEditor;
using UnityEngine;

namespace CustomAttributes
{

    [CustomPropertyDrawer(typeof(LocalComponentAttribute))]
    public class LocalComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LocalComponentAttribute localComponentAttribute = attribute as LocalComponentAttribute;

            bool multiEdit = property.serializedObject.isEditingMultipleObjects;
            if (multiEdit)
            {
                foreach (var obj in property.serializedObject.targetObjects)
                {
                    if (obj == null)
                        continue;

                    var serObj = new SerializedObject(obj);
                    var prop = serObj.FindProperty(property.propertyPath);


                    if (prop == null)
                        continue;

                    AssignValues(prop, localComponentAttribute);

                    serObj.ApplyModifiedProperties();
                }
            }
            else
            {
                AssignValues(property, localComponentAttribute);
            }

            bool wasEnabled = GUI.enabled;

            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                if (localComponentAttribute.lockProperty)
                    GUI.enabled = false;

                if (!localComponentAttribute.hideProperty)
                    EditorGUI.PropertyField(position, property, label);


                if (multiEdit && property.hasMultipleDifferentValues)
                    EditorGUI.showMixedValue = true;
            }


            GUI.enabled = wasEnabled;
        }

        private void AssignValues(SerializedProperty property, LocalComponentAttribute localComponentAttribute)
        {
            if (property.objectReferenceValue != null)
                return;

            GameObject mono = null;
            if (localComponentAttribute.parentObject != null && localComponentAttribute.parentObject != "")
            {
                string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
                string conditionPath = propertyPath.Replace(property.name, localComponentAttribute.parentObject); //changes the path to the conditionalsource property path
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                if (sourcePropertyValue != null)
                    mono = sourcePropertyValue.objectReferenceValue as GameObject;
                if (mono == null)
                {
                    property.objectReferenceValue = null;
                    return;
                }
                if (sourcePropertyValue == null)
                {
                    Debug.LogError("Field " + fieldInfo.Name + " doesn't exist!");
                    return;
                }

            }
            else
                mono = (property.serializedObject.targetObject as MonoBehaviour).gameObject;
            //if(fieldInfo.FieldType.IsSubclassOf(typeof(Component)) )
            if (typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
            {
                if (property.objectReferenceValue == null)
                {
                    Component comp = null;
                    bool includeInactive = localComponentAttribute.includeInactive;
                    switch (localComponentAttribute.getComponentFromChildrens)
                    {
                        default:
                        case GetComponentTargets.Local:
                            comp = mono.GetComponent(fieldInfo.FieldType);
                            break;
                        case GetComponentTargets.Childrens:
                            comp = mono.GetComponentInChildren(fieldInfo.FieldType, includeInactive);
                            break;
                        case GetComponentTargets.Parents:
#if UNITY_2021_2_OR_NEWER
                            comp = mono.GetComponentInParent(fieldInfo.FieldType, includeInactive);
#else
                            comp = mono.GetComponentInParent(fieldInfo.FieldType);
#endif
                            break;
                        case GetComponentTargets.Anywhere:
                            var roots = mono.scene.GetRootGameObjects();
                            foreach (var root in roots)
                            {
                                comp = root.GetComponentInChildren(fieldInfo.FieldType, includeInactive);

                                if (comp != null)
                                    break;
                            }
                            break;
                    }

                    property.objectReferenceValue = comp;
                }
            }
            else
            {
                Debug.LogError("Field <b>" + fieldInfo.Name + "</b> of " + mono.GetType() + " is not a component!", mono);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            LocalComponentAttribute localComponentAttribute = attribute as LocalComponentAttribute;

            if (property.objectReferenceValue == null || !localComponentAttribute.hideProperty)
                return EditorGUI.GetPropertyHeight(property, label);
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}

