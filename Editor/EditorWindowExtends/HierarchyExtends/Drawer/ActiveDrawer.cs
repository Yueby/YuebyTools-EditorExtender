using UnityEditor;
using UnityEngine;
using Yueby.Core.Utils;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Logger = Yueby.Core.Utils.Logger;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class ActiveDrawer : HierarchyDrawer
    {
        protected override int DefaultOrder => 1;
        private readonly DrawerIconContent _visibleIconContent =
            new(
                EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x"),
                EditorGUIUtility.IconContent("animationvisibilitytoggleoff@2x")
            );

        // private DrawerIconContent _visibleHoverIconContent = new(EditorGUIUtility.IconContent("animationvisibilitytoggleon@2x"), EditorGUIUtility.IconContent("animationvisibilitytoggleoff@2x"));

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);
            if (selectionItem.TargetObject == null)
                return;
            // if (!selectionItem.IsHover) return;
            // if (selectionItem.TargetObject.activeSelf)
            // {
            //     if (!selectionItem.IsHover) return;
            // }

            var height = EditorGUIUtility.singleLineHeight;
            DrawControl(
                selectionItem,
                height + 2,
                height,
                (rect) =>
                {
                    rect.y += 1;

                    rect.x += 2;
                    rect.y -= 0.5f;

                    var iconSize = EditorGUIUtility.GetIconSize();

                    EditorGUI.BeginDisabledGroup(
                        !selectionItem.TargetObject.activeSelf
                            || !selectionItem.TargetObject.activeInHierarchy
                    );
                    EditorGUIUtility.SetIconSize(new Vector2(height - 2, height - 2));
                    GUI.Label(
                        rect,
                        selectionItem.TargetObject.activeSelf
                            ? _visibleIconContent.Light
                            : _visibleIconContent.Dark
                    );
                    EditorGUIUtility.SetIconSize(iconSize);
                    EditorGUI.EndDisabledGroup();

                    if (
                        Event.current.type == EventType.MouseDown
                        && rect.Contains(Event.current.mousePosition)
                    )
                    {
                        Extender.ActiveObjectHandler = new ActiveObjectHandler()
                        {
                            Active = !selectionItem.TargetObject.activeSelf,
                            LastGameObject = null,
                        };
                    }
                    else if (
                        Event.current.type == EventType.MouseUp
                        && Extender.ActiveObjectHandler != null
                    )
                    {
                        Extender.ActiveObjectHandler = null;
                    }

                    if (rect.Contains(Event.current.mousePosition))
                    {
                        if (
                            Extender.ActiveObjectHandler != null
                            && Extender.ActiveObjectHandler.LastGameObject
                                != selectionItem.TargetObject
                        )
                        {
                            selectionItem.TargetObject.SetActive(
                                Extender.ActiveObjectHandler.Active
                            );
                            Event.current.Use();
                            Extender.ActiveObjectHandler.LastGameObject =
                                selectionItem.TargetObject;
                        }
                    }
                }
            );
        }
    }
}
