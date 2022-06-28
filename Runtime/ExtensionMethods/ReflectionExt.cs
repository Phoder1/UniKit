using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Compilation;

namespace UniKit.Reflection
{
    public static class ReflectionExt
    {
        public const BindingFlags DefaultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        public static FieldInfo GetObjectField(this object obj, string name, bool caseSensitive = true)
        {
            var flags = DefaultFlags;

            if (!caseSensitive)
                flags |= BindingFlags.IgnoreCase;

            Type type = obj.GetType();
            return type.GetField(name, flags);
        }
        public static PropertyInfo GetObjectProperty(this object obj, string name, bool caseSensitive = true)
        {
            var flags = DefaultFlags;

            if (!caseSensitive)
                flags |= BindingFlags.IgnoreCase;

            Type type = obj.GetType();
            var property = type.GetProperty(name, flags);
            var properties = type.GetProperties();
            return property;
        }
        public static MethodInfo GetObjectMethod(this object obj, string name, bool caseSensitive = true)
        {
            var flags = DefaultFlags;

            if (!caseSensitive)
                flags |= BindingFlags.IgnoreCase;

            Type type = obj.GetType();
            return type.GetMethod(name, flags);
        }
        public static bool TryGetMemberValue<T>(this object obj, string name, out T value, bool caseSensitive = true)
        {
            var field = obj.GetObjectField(name, caseSensitive);
            if (field != null && field.FieldType == typeof(T))
            {
                value = (T)field.GetValue(obj);
                return true;
            }

            var property = obj.GetObjectProperty(name, caseSensitive);
            if (property != null && property.PropertyType == typeof(T))
            {
                value = (T)property.GetValue(obj);
                return true;
            }

            var method = obj.GetObjectMethod(name, caseSensitive);
            if (method != null && method.ReturnType == typeof(T) && method.GetParameters().Length == 0)
            {
                value = (T)method.Invoke(obj, null);
                return true;
            }

            value = default;
            return false;
        }
        public static bool TryFindField(
            this Type type,
            string name,
            out FieldInfo field)
            => type.TryFindField(name, DefaultFlags, out field);

        public static bool TryFindField(
            this Type type,
            string name,
            BindingFlags flags,
            out FieldInfo field)
        {
            do
            {
                field = type.GetFields(flags)
                    .FirstOrDefault(f => f.Name == name);
                if (field != default)
                    return true;
                else
                    type = type.BaseType;
            } while (type != null);

            return false;
        }
        public static bool TryGetFieldValue<TIn, TOut>(
            this TIn obj,
            string fieldName,
            out TOut value)
        {
            value = default;

            if (!typeof(TIn).TryFindField(fieldName, out var field))
                return false;

            if (field == null || !(field.GetValue(obj) is TOut outVal))
                return false;

            value = outVal;
            return true;
        }

        public static bool TryGetPropertyValue<TIn, TOut>(
            this TIn obj,
            string propertyName,
            out TOut value)
        {

            if (typeof(TIn).TryFindProperty(propertyName, out var property)
                && property != null
                && property.GetValue(obj) is TOut outVal)
            {
                value = outVal;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryFindProperty(
            this Type type,
            string name,
            out PropertyInfo property)
            => type.TryFindProperty(name, DefaultFlags, out property);

        public static bool TryFindProperty(
            this Type type,
            string name,
            BindingFlags flags,
            out PropertyInfo property)
        {
            do
            {
                property = type.GetProperties(flags)
                    .FirstOrDefault(f => f.Name == name);
                if (property != default)
                    return true;
                else
                    type = type.BaseType;
            } while (type != null);

            return false;
        }

        public static bool TryGetPropertyOrFieldValue<TIn>(
            this TIn obj,
            string name,
            out object value,
            out Type type)
        {
            PropertyInfo property;
            FieldInfo field;
            Type inType = typeof(TIn);
            do
            {
                property = inType.GetProperty(name, DefaultFlags);

                if (property != null)
                {
                    value = property.GetValue(obj);
                    type = property.PropertyType;
                    return true;
                }

                field = inType.GetField(name, DefaultFlags);

                if (field != null)
                {
                    value = field.GetValue(obj);
                    type = field.FieldType;
                    return true;
                }

                inType = inType.BaseType;
            } while (inType != null);

            value = default;
            type = default;
            return false;
        }
        public static bool TryGetPropertyOrFieldValue<TIn, TOut>(
            this TIn obj,
            string name,
            out TOut value)
        {
            PropertyInfo property;
            FieldInfo field;
            Type type = typeof(TIn);
            do
            {
                property = type.GetProperty(name, DefaultFlags);

                if (property != null)
                {
                    if (property.GetValue(obj) is TOut outVal)
                    {
                        value = outVal;
                        return true;
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }

                field = type.GetField(name, DefaultFlags);

                if (field != null)
                {
                    if (field.GetValue(obj) is TOut outVal)
                    {
                        value = outVal;
                        return true;
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }

                type = type.BaseType;
            } while (type != null);

            value = default;
            return false;
        }

        public static bool TryFindPropertyOrField(
            this Type type,
            string name,
            out MemberInfo member)
            => type.TryFindPropertyOrField(name, DefaultFlags, out member);

        public static bool TryFindPropertyOrField(
            this Type type,
            string name,
            BindingFlags flags,
            out MemberInfo member)
        {
            if (type.TryFindProperty(name, flags, out var property))
            {
                member = property;
                return true;
            }
            if (type.TryFindField(name, flags, out var field))
            {
                member = field;
                return true;
            }
            member = null;
            return false;
        }
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, UnityEditor.Compilation.Assembly referencedAssembly)
            => assembly.HasReferenceTo(referencedAssembly.name);
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, System.Reflection.Assembly referencedAssembly)
            => assembly.HasReferenceTo(referencedAssembly.FullName);
        public static bool HasReferenceTo(this UnityEditor.Compilation.Assembly assembly, string referencedAssembly)
            => Array.Exists(assembly.assemblyReferences, (x) => referencedAssembly.StartsWith(x.name));

        public static List<UnityEditor.Compilation.Assembly> GetAllThatMightImplement(this Type type)
        {
            var assembly = type.Assembly;
            var allAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            return allAssemblies.Where((x) => x.HasReferenceTo(assembly) || assembly.FullName.StartsWith(x.name));
        }
        public static IEnumerable<System.Reflection.Assembly> GetSystemAssemblies(this IEnumerable<UnityEditor.Compilation.Assembly> assemblies)
        {
            var allSystemAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in allSystemAssemblies)
                if (assemblies.Any((x) => assembly.FullName.StartsWith(x.name)))
                    yield return assembly;
        }
        public static bool IsAssignableFromGeneric(this Type baseType, Type extendType)
        {
            if (!baseType.IsGenericType)
                return baseType.IsAssignableFrom(extendType);

            var currentType = extendType;
            while (!baseType.IsAssignableFrom(currentType))
            {
                if (currentType == null)
                    return false;

                if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
                    currentType = currentType.GetGenericTypeDefinition();
                else
                    currentType = currentType.BaseType;
            }

            return true;
        }
    }
}