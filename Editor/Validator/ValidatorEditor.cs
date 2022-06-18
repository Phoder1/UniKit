using Phoder1.Core.QA;
using Phoder1.Core.Types;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Phoder1.Editor.QA
{
    public static class ValidatorEditor
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitValidator()
        {
            SceneManager.sceneLoaded += AutomatedSceneValidityCheck;

            void AutomatedSceneValidityCheck(Scene scene, LoadSceneMode loadSceneMode) => SceneValidator.ValidateScene(scene);
        }

        [MenuItem("Phoder1/Validate Scene/Output Last Log")]
        public static void OutputLastLog()
            => SceneValidator.OutputLastLog();

        // Add menu item
        [MenuItem("CONTEXT/MonoBehaviour/Validate")]
        static void Validate(MenuCommand command)
        {
            var context = command.context;
            if (context is IValidateable validateable)
            {
                var valid = validateable.IsValid();
                var msg = "IsValid returned: " + (valid ? "Valid!" : valid.ErrorMessage);
                SceneValidator.DebugLog(msg, validateable as Component);
            }

            if (context.GetType().GetCustomAttribute<NullReferenceValidate>() != null)
            {
                var valid = SceneValidator.NullReferencesReflectionValidate(context);
                var msg = "Reflection validator returned: " + (valid ? "Valid!" : valid.ErrorMessage);
                SceneValidator.DebugLog(msg, context as Component);
            }
        }
        [MenuItem("CONTEXT/MonoBehaviour/Validate", validate = true)]
        static bool IsValidToValidate(MenuCommand command)
            => command.context is IValidateable
            || command.context.GetType().GetCustomAttribute<NullReferenceValidate>() != null;

        [MenuItem("Phoder1/Validate Scene/Validate Current Scene")]
        public static Result ValidateCurrentScene()
            => SceneValidator.ValidateCurrentScene(true);
    }
}
