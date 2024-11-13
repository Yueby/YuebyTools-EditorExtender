using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;
using Yueby.EditorWindowExtends.Utils;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Drawer
{
    public class SeparatorDrawer : HierarchyDrawer
    {
        public override string DrawerName => "Separator";
        protected override int DefaultOrder => 100;

        public override void OnHierarchyWindowItemGUI(SelectionItem selectionItem)
        {
            base.OnHierarchyWindowItemGUI(selectionItem);

            if (selectionItem.OriginRect.height <= EditorGUIUtility.singleLineHeight)
            {
                var rect = new Rect(selectionItem.OriginRect.x, selectionItem.OriginRect.y + selectionItem.OriginRect.height - 1f, selectionItem.OriginRect.width + selectionItem.OriginRect.x, 1);
                EditorGUI.DrawRect(rect, Styles.SeparatorColor.GetColor());
            }
        }
    }
}