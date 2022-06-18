using Phoder1.Core.Types;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Phoder1.Editor
{
    public class DefinesStream : IDisposable
    {
        bool disposed = false;
        bool changed = false;
        readonly BuildTargetGroup currentTarget;
        readonly List<string> defines;

        public DefinesStream()
        {
            currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (currentTarget == BuildTargetGroup.Unknown)
                return;

            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Trim();
            defines = new List<string>(definesString.Split(';'));
        }
        public Result AddDefine(string define)
        {
            if (Exists(define))
                return Result.Failed("Define already exists");

            defines.Add(define);
            changed = true;

            return Result.Success();
        }
        public Result RemoveDefine(string define)
        {
            if (!defines.Remove(define))
                return Result.Failed("Define isn't set");

            changed = true;
            return Result.Success();

        }
        public Result Exists(string define)
            => defines.Contains(define);
        public void Flush()
        {
            if (!changed)
                return;

            var definesString = string.Join(';', defines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
            AssetDatabase.SaveAssets();
        }

        ~DefinesStream()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Flush();
        }
    }
}
