using UnityEditor;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class HeaderDrawer : HierarchyDrawer
    {
        public override int DefaultOrder => 2;

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);

            if (selectionItem.TargetObject == null) return;

            var name = selectionItem.TargetObject.name;
            if (name.StartsWith("#h"))
            {
                var headerName = name[2..];
                var rect = selectionItem.OriginRect;

                EditorGUI.DrawRect(rect, Styles.HierarchyBackgroundColor.GetColor());
                EditorGUI.LabelField(rect, headerName, Styles.HeaderStyle);
            }

        }
    }
}