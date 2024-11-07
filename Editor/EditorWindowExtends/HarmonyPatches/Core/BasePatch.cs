using System;
using HarmonyLib;
using UnityEngine;
using Yueby.Core.Utils;
using Logger = Yueby.Core.Utils.Logger;

namespace Yueby.EditorWindowExtends.HarmonyPatches.Core
{
    public abstract class BasePatch
    {
        protected bool IsEnabled { get; private set; }

        protected virtual string PatchId => GetType().Name;

        public virtual void OnEnabled() { }

        public virtual void OnDisabled() { }

        protected abstract void ApplyPatch(Harmony harmony);

        protected virtual void HandlePatchError(Exception ex)
        {
            Logger.LogException(ex, $"[{PatchId}] Patch failed");
            IsEnabled = false;
        }

        internal void Apply(Harmony harmony)
        {
            if (IsEnabled)
                return;

            try
            {
                ApplyPatch(harmony);
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
                Logger.LogException(ex, $"[{PatchId}] Cleanup failed");
            }
        }
    }
}
