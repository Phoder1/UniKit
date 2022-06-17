using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phoder1.Core.QA
{
    public class IgnoreValidationAttribute : Attribute { }
    public class ValidateArrayAttribute : Attribute { }
    public interface IReflectionValidateable { }
    /// <summary>
    /// Used to check if a component is properly assigned, mainly used inside SceneValidator check (both ingame and in editor!)
    /// </summary>
    public interface IValidateable
    {

        /// <summary>
        /// Used to check if a component is properly assigned, mainly used inside SceneValidator check (both ingame and in editor!)
        /// </summary>
        /// <param name="InvalidReasons">If invalid, all the reasons</param>
        /// <returns>Whether the class is valid or not</returns>
        bool IsValid(out string[] InvalidReasons);
    }
    public static class SceneValidator
    {
        private static Scene _lastSceneChecked;
        private static List<InvalidLogReport> _reports;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitValidator()
        {
            SceneManager.sceneLoaded += AutomatedSceneValidityCheck;

            void AutomatedSceneValidityCheck(Scene scene, LoadSceneMode loadSceneMode) => ValidateScene(scene);
        }


        [UnityEditor.MenuItem("MarGish/Validate Scene/Validate Current Scene")]
        public static bool ValidateCurrentScene()
            => ValidateCurrentScene(true);
#endif
        public static bool ValidateCurrentScene(bool OutputLogs = true)
            => ValidateScene(SceneManager.GetActiveScene(), OutputLogs);
        public static bool ValidateScene(Scene scene, bool OutputLogs = true)
        {
            bool _isValid = true;
            if (_reports == null)
                _reports = new List<InvalidLogReport>();
            else
                _reports?.Clear();

            _lastSceneChecked = scene;

            if (!scene.IsValid())
            {
                Log(logMessages: "Scene is invalid!");
                return false;
            }

            foreach (var validateable in FindAllInScene<IValidateable>(scene))
                if (!validateable.IsValid(out string[] reasons))
                    Log(validateable as Component, reasons);

            foreach (var reflectionValidateable in FindAllInScene<IReflectionValidateable>(scene))
                if (!reflectionValidateable.NullReferencesReflectionValidator(out string[] reasons))
                    Log(reflectionValidateable as Component, reasons);

            if (OutputLogs)
                OutputLastLog();

            return _isValid;

            void Log(Component context = null, params string[] logMessages)
            {
                _isValid = false;
                _reports.Add(new InvalidLogReport(context, logMessages));
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("MarGish/Validate Scene/Output Last Log")]
#endif
        public static void OutputLastLog()
        {
            if (_lastSceneChecked == null || _lastSceneChecked == null || string.IsNullOrEmpty(_lastSceneChecked.name))
                DebugLog("No log to display");
            else if (_reports == null || _reports.Count <= 0)
                DebugLog($"Scene <b>{_lastSceneChecked.name}</b> is Valid!");
            else
            {
                string message =
                    $"<color=#b3b300ff>Scene <b>{_lastSceneChecked.name}</b> is invalid!</color>\n"
                    + string.Join(
                        "\n",
                        _reports.ConvertAll((x) => $"<color=yellow><b>{x.Object.GetPath()}</b></color>:\n- <i>{string.Join("\n- ", x.InvalidReasons)}</i>")
                        )
                    + "\n\n\n\n";
                DebugLog(message, _reports[0].Object, LogType.Warning);
            }
        }
        private static void DebugLog(string message, UnityEngine.Object context = null, LogType logType = LogType.Log)
            => Debug.unityLogger.Log(logType, "[SceneValidator]", message, context);

        public static bool NullReferencesReflectionValidator(this IReflectionValidateable toValidate, out string[] invalidReasons)
            => NullReferencesReflectionValidator(toValidate as object, out invalidReasons);
        public static bool NullReferencesReflectionValidator(this IValidateable toValidate, out string[] invalidReasons)
            => NullReferencesReflectionValidator(toValidate as object, out invalidReasons);
        public static bool NullReferencesReflectionValidator(object toValidate, out string[] invalidReasons)
        {
            var nullFields = GetNullFields(toValidate).ToArray();

            if (nullFields == null || nullFields.Length == 0)
            {
                invalidReasons = default;
                return true;
            }

            invalidReasons = Array.ConvertAll(nullFields, (x) => $"{x.Name.ToDisplayName()} is not assigned!");
            return false;
        }
        private static IEnumerable<FieldInfo> GetNullFields(object obj)
        {
            var fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field == null
                    || !field.GetType().IsSerializable
                    || field.GetCustomAttribute<IgnoreValidationAttribute>() != null)
                    continue;

                //Is serielized by Unity?
                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                    continue;

                var value = field.GetValue(obj);
                if (value == null || value.Equals(null))
                {
                    yield return field;
                }
                else if (field.GetCustomAttribute<ValidateArrayAttribute>() != null && value is IList collection)
                {
                    for (int i = 0; i < collection.Count; i++)
                        if (collection[i] == null)
                        {
                            yield return field;
                            break;
                        }
                }
            }
        }

        public static bool NullReferencesReflectionValidator(this IValidateable validateable, out string[] invalidReasons, params string[] exactObjectsNames)
        {
            var reasons = Reasons();
            if (reasons == null || !reasons.Any())
            {
                invalidReasons = default;
                return true;
            }

            invalidReasons = reasons.ToArray();
            return false;

            IEnumerable<string> Reasons()
            {
                for (int i = 0; i < exactObjectsNames.Length; i++)
                {
                    var type = validateable.GetType();
                    if (string.IsNullOrWhiteSpace(exactObjectsNames[i]))
                        continue;

                    var field = type.GetField(exactObjectsNames[i], BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field == null || field.GetValue(validateable) != null)
                        continue;

                    yield return $"{field.Name} is not assigned!";
                }
            }
        }

        public static bool Validator(this IValidateable validateable, out string[] invalidReasons, params (Func<bool> condition, string message)[] input)
        {
            List<string> reasons = new List<string>();

            foreach (var item in input)
                if (item.condition?.Invoke() ?? false)
                    reasons.Add(item.message);


            if (reasons == null || reasons.Count == 0)
            {
                invalidReasons = default;
                return true;
            }

            invalidReasons = reasons.ToArray();
            return false;
        }
        private static IEnumerable<T> FindAllInScene<T>(Scene scene)
        {
            var rootObjects = scene.GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                var validateables = rootObjects[i].GetComponentsInChildren<T>();

                if (validateables != null && validateables.Length > 0)
                    foreach (var x in validateables)
                        yield return x;
            }
        }
        private readonly struct InvalidLogReport
        {
            public readonly Component Object;
            public readonly IReadOnlyList<string> InvalidReasons;

            public InvalidLogReport(Component @object, IReadOnlyList<string> invalidReason)
            {
                Object = @object;
                InvalidReasons = invalidReason;
            }
        }
        public static string GetPath(this Transform current)
            => current.parent == null
            ? current.name
            : current.parent.GetPath() + "/" + current.name;
        public static string GetPath(this Component component)
            => component.transform.parent == null
            ? component.name + "/" + component.GetType().Name
            : component.transform.parent.GetPath() + "/" + component.name + "/" + component.GetType().Name;
    }
}
