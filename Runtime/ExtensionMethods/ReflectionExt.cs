using System;
using System.Linq;
using System.Reflection;

namespace Phoder1.Reflection
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
    }
}