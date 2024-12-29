using System;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Yueby.Core.Utils;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Core
{
    public abstract class BasePatch
    {
        protected bool IsEnabled { get; private set; }

        protected virtual string PatchId => GetType().Name;

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }

        protected abstract Task ApplyPatch(Harmony harmony);

        protected virtual void HandlePatchError(Exception ex)
        {
            YuebyLogger.LogException(ex, $"[{PatchId}] Patch failed");
            IsEnabled = false;
        }

        internal async Task Apply(Harmony harmony)
        {
            if (IsEnabled)
                return;

            try
            {
                await ApplyPatch(harmony);
                IsEnabled = true;
                OnEnabled();
            }
            catch (Exception ex)
            {
                HandlePatchError(ex);
                throw;
            }
        }

        public virtual void Cleanup()
        {
            if (!IsEnabled)
                return;

            try
            {
                OnDisabled();
                IsEnabled = false;
            }
            catch (Exception ex)
            {
                YuebyLogger.LogException(ex, $"[{PatchId}] Cleanup failed");
            }
        }
    }
}
