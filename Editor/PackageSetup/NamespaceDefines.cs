using System;
using System.IO;
using System.Text;
using UnityEditor;
using static Newtonsoft.Json.JsonConvert;

namespace Phoder1.Editor
{
    public static class NamespaceDefines
    {
        private const string FILE_COMMENT
            = "For each group the package will check if the namespace exists,\n" +
            "and if it does will add the appropriate define to the build settings";
        private const string FILE_NAME = "NamespaceDefines.json";
        private static string PackagePath
            = typeof(NamespaceDefines)
            .Assembly
            .FindContainingPackage()
            .resolvedPath;

        private static string FilePath
            => Path.Combine(PackagePath, FILE_NAME);

        [InitializeOnLoadMethod]
        private static void AddNamespaceDefines()
        {
            ValidateFileExists();
            Define();
        }

        private static void Define()
        {
            string file = File.ReadAllText(FilePath);
            DefineSettings settings = DeserializeObject<DefineSettings>(file);

            if (settings.defines == null || settings.defines.Length == 0)
                return;

            using (var stream = new DefinesStream())
            {
                foreach (var namespaceDefine in settings.defines)
                {
                    if (string.IsNullOrWhiteSpace(namespaceDefine.Namespace) || string.IsNullOrWhiteSpace(namespaceDefine.Define))
                        continue;

                    string defineName
                        = namespaceDefine.Define
                        .RemoveSpecialCharacters()
                        .SpaceToUnderScore()
                        .RemoveAllWhitespaces();

                    if (stream.Exists(defineName))
                        continue;

                    if (!NamespaceExists(namespaceDefine.Namespace))
                        continue;

                    stream.AddDefine(defineName);
                }
            }
        }
        private static bool NamespaceExists(string namespaceName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var type in assembly.GetTypes())
                    if (type.Namespace == namespaceName)
                        return true;

            return false;
        }
        private static void ValidateFileExists()
        {
            if (File.Exists(FilePath))
                return;
            var comment = $"/*\n{FILE_COMMENT}\n*/\n";

            var settings = new DefineSettings(new NamespaceDefine[1] { new NamespaceDefine("", "") });
            var settingsStr = comment + SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            using (var stream = File.Create(FilePath))
            {

                stream.Write(Encoding.UTF8.GetBytes(settingsStr));
            }

        }

        [Serializable]
        private struct DefineSettings
        {
            public NamespaceDefine[] defines;

            public DefineSettings(NamespaceDefine[] defines)
            {
                this.defines = defines;
            }
        }
        [Serializable]
        private struct NamespaceDefine
        {
            public string Namespace;
            public string Define;

            public NamespaceDefine(string @namespace, string define)
            {
                Namespace = @namespace;
                Define = define;
            }
        }
    }
}
