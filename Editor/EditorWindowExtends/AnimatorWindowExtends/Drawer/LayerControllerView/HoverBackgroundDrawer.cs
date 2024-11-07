using UnityEditorInternal;
using UnityEngine;
using Yueby.EditorWindowExtends.AnimatorWindowExtends.Core;
using Yueby.EditorWindowExtends.Reflections;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.AnimatorWindowExtends.Drawer.LayerControllerView
{
    public class HoverBackgroundDrawer : LayerControllerViewDrawer
    {
        public override string DrawerName => "Hover Background";

        public override string Tooltip => "Draws a background when hovering over an item in the layer list";
        // public static GUIStyle hoveredItemBackgroundStyle = (GUIStyle) "WhiteBackground";
        // public static Color hoveredBackgroundColor = EditorResources.Load("game-object-tree-view").GetColor("-unity-object-tree-hovered-color");

        public override void Init(LayerControllerViewExtender extender, ReorderableList reorderableList)
        {
            base.Init(extender, reorderableList);
        }

        public override void OnDrawElementBackground(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (rect.Contains(Event.current.mousePosition) && !isactive)
                using (new BackgroundColorScope(GameObjectStylesReflect.GetHoveredBackgroundColor()))
                {
                    GUI.Label(rect, GUIContent.none, GameObjectStylesReflect.GetHoveredItemBackgroundStyle());
                }
        }
    }
}