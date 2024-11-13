using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.HarmonyPatches.Core;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public static class PatchLoader
    {
        private static readonly Dictionary<Type, BasePatch> _patches = new();
        private static Harmony _harmony;
        private static int _initializedCount;

        [InitializeOnLoadMethod]
        internal static void PrepareApplyPatches()
        {
            RegisterAllPatches();
            EditorApplication.update += OnUpdate;
        }

        private static async void OnUpdate()
        {
            try
            {
                EditorApplication.update -= OnUpdate;
                await Task.Delay(TimeSpan.FromSeconds(1f));
                await ApplyPatches();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to apply patches");
            }
        }

        private static void RegisterAllPatches()
        {
            var patchTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(BasePatch).IsAssignableFrom(t));

            foreach (var type in patchTypes)
            {
                RegisterPatch(type);
            }
        }

        public static void RegisterPatch(Type patchType)
        {
            if (!typeof(BasePatch).IsAssignableFrom(patchType))
                throw new ArgumentException($"Type {patchType} is not a BasePatch");

            if (!_patches.ContainsKey(patchType))
                _patches[patchType] = (BasePatch)Activator.CreateInstance(patchType);
        }

        public static void EnablePatch<T>()
            where T : BasePatch
        {
            if (_patches.TryGetValue(typeof(T), out var patch))
                patch.OnEnabled();
        }

        public static void DisablePatch<T>()
            where T : BasePatch
        {
            if (_patches.TryGetValue(typeof(T), out var patch))
                patch.OnDisabled();
        }

        public static void RegisterPatch<T>()
            where T : BasePatch, new()
        {
            var type = typeof(T);
            if (!_patches.ContainsKey(type))
                _patches[type] = new T();
        }

        internal static async Task ApplyPatches()
        {
            _harmony = new Harmony("yueby.tools.core");

            RegisterPatch<ProjectBrowserPatch>();
            RegisterPatch<AnimatorWindowPatch>();

            foreach (var patch in _patches.Values)
            {
                try
                {
                    await patch.Apply(_harmony);
                    _initializedCount++;
                }
                catch (Exception)
                {
                    _initializedCount--;
                }
            }

            if (_initializedCount == _patches.Count)
                Logger.LogInfo("All patches applied.");

            AssemblyReloadEvents.beforeAssemblyReload -= UnpatchAll;
            AssemblyReloadEvents.beforeAssemblyReload += UnpatchAll;
        }

        internal static void UnpatchAll()
        {
            Logger.LogInfo("Unpatching all patches");
            _harmony?.UnpatchAll();
        }
    }
}
