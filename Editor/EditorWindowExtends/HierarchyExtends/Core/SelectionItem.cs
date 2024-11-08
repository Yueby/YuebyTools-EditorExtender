using UnityEngine;
using UnityEditor;

namespace Yueby.EditorWindowExtends.HierarchyExtends.Core
{
    public class SelectionItem
    {
        public Rect Rect;
        public Rect OriginRect;

        public int InstanceID { get; private set; }
        public bool IsHover { get; private set; }

        public GameObject TargetObject { get; private set; }

        public SelectionItem(int instanceID, Rect rect)
        {
            InstanceID = instanceID;
            OriginRect = rect;
            Rect = rect;
            TargetObject = EditorUtility.InstanceIDToObject(InstanceID) as GameObject;
        }

        public void Refresh(int instanceID, Rect rect)
        {
            OriginRect = rect;
            Rect = rect;
            InstanceID = instanceID;
            IsHover = rect.Contains(Event.current.mousePosition);
        }
    }
}