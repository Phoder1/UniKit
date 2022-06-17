﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorExtensionMethods
{
    public static void FlatProperty(this SerializedProperty property)
    {
        foreach (var child in property.GetChildren())
            EditorGUILayout.PropertyField(child);
    }

    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
    {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.NextVisible(false);
        if (!hasNextElement)
        {
            nextElement = null;
        }

        property.NextVisible(true);

        while (true)
        {
            if ((SerializedProperty.EqualContents(property, nextElement)))
            {
                yield break;
            }

            yield return property;

            bool hasNext = property.NextVisible(false);
            if (!hasNext)
            {
                break;
            }
        }
    }
    public static bool GetIsArrayElement(this SerializedProperty property)
        => property.propertyPath.Contains('[') && property.propertyPath.Contains(']');
    public static int GetArrayPropertyIndex(this SerializedProperty property)
        => int.Parse(property.propertyPath.Split('[').LastOrDefault().TrimEnd(']'));
    public static T GetTarget<T>(this PropertyDrawer drawer, SerializedProperty property)
    {
        T myDataClass = (T)drawer.fieldInfo.GetValue(property.serializedObject.targetObject);
        return myDataClass;
    }
    public static object GetTarget(this PropertyDrawer drawer, SerializedProperty property)
    {
        object myDataClass = drawer.fieldInfo.GetValue(property.serializedObject.targetObject);
        return myDataClass;
    }

    public static FieldInfo GetField(this SerializedProperty property)
    {
        BindingFlags bindings = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        UnityEngine.Object targetObj = property.serializedObject.targetObject;

        FieldInfo field = null;

        string[] name = property.propertyPath.Split('.');
        Type type = targetObj.GetType();

        for (int i = 0; i < name.Length; i++)
        {
            field = type.GetField(name[i], bindings);
            type = field.FieldType;
        }

        return field;
    }

    public static void AsRefField(this ref int value, string name = null, bool guiEnabled = true)
        => value = AsField(value, name, guiEnabled);
    public static int AsField(this int value, string name = null, bool guiEnabled = true)
    {
        bool guiTemp = GUI.enabled;
        GUI.enabled = guiEnabled;

        int temp;

        if (name == null)
            temp = EditorGUILayout.IntField(value);
        else
            temp = EditorGUILayout.IntField(name, value);

        GUI.enabled = guiTemp;

        return temp;
    }
    public static Object AsField(this Object value, string name = null, bool guiEnabled = true)
    {
        bool guiTemp = GUI.enabled;
        GUI.enabled = guiEnabled;

        Object temp;

        if (name == null)
            temp = EditorGUILayout.ObjectField(value, value.GetType(), true);
        else
            temp = EditorGUILayout.ObjectField(name, value, value.GetType(), true);

        GUI.enabled = guiTemp;

        return temp;
    }
    public static void AsHelpBox(this string value, MessageType type = MessageType.None, bool wide = true)
    {
        EditorGUILayout.HelpBox(value, type, wide);
    }
    public static string AsField(this string value, string name = null, bool guiEnabled = true)
    {
        bool guiTemp = GUI.enabled;
        GUI.enabled = guiEnabled;

        string temp;

        if (name == null)
            temp = EditorGUILayout.TextField(value);
        else
            temp = EditorGUILayout.TextField(name, value);

        GUI.enabled = guiTemp;

        return temp;
    }
}