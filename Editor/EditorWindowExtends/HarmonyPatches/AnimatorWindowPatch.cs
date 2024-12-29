using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.AnimatorWindowExtends;
using Yueby.EditorWindowExtends.AnimatorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.HarmonyPatches.Core;
using Yueby.EditorWindowExtends.HarmonyPatches.MapperObject;
using Yueby.Utils.Reflections;
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

        protected override async Task ApplyPatch(Harmony harmony)
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
                YuebyLogger.LogException(ex, "Failed to map GraphGUI");
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

        private static void OnOnDrawStateNode(GraphGUI arg1, StateNode arg2)
        {
            OnDrawStateNode?.Invoke(arg1, arg2);
        }
    }
}