using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorWindowExtends;
using Yueby.EditorWindowExtends.AnimatorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.HarmonyPatches.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;
using Yueby.Utils.Reflections;
using Logger = Yueby.Core.Utils.Logger;
using Object = UnityEngine.Object;

namespace Yueby.EditorWindowExtends.HarmonyPatches
{
    public class AnimatorWindowPatch : BasePatch
    {
        private static readonly HashSet<string> NodeIgnore =
            new(new[] { "Any State", "Entry", "Exit" });

        private static readonly Dictionary<int, StateNode> _stateNodeCaches = new();
        private static GraphGUI _graphGUI;

        public static event Action<GraphGUI, StateNode> OnDrawStateNode;

        protected override void ApplyPatch(Harmony harmony)
        {
            var onGraphGUIMethodPrefix = AccessTools.Method(
                typeof(AnimatorWindowPatch),
                nameof(OnGraphGUIPrefix)
            );
            var onGraphGUIMethodPostfix = AccessTools.Method(
                typeof(AnimatorWindowPatch),
                nameof(OnGraphGUIPostfix)
            );

            harmony.Patch(
                AnimatorWindowReflect.GraphGUIType.Method("OnGraphGUI"),
                new HarmonyMethod(onGraphGUIMethodPrefix),
                new HarmonyMethod(onGraphGUIMethodPostfix)
            );
        }

        private static void OnGraphGUIPrefix(Object __instance)
        {
            if (__instance == null)
                return;

            try
            {
                _graphGUI = Mapper.Map<GraphGUI>(__instance);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Failed to map GraphGUI");
            }
        }

        private static void OnGraphGUIPostfix()
        {
            if (_graphGUI == null)
                return;

            foreach (var node in _graphGUI.Graph.nodes)
            {
                if (NodeIgnore.Contains(node.Title))
                    continue;
                GraphGUIExtender.OnDrawStateNode(_graphGUI, node);
            }
        }

        public override void OnDisabled()
        {
            _stateNodeCaches.Clear();
            OnDrawStateNode = null;
        }
    }
}