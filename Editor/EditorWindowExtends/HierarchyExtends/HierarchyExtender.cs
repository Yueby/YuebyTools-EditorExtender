using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Yueby.EditorWindowExtends.Core;
using Yueby.EditorWindowExtends.HierarchyExtends.Core;

namespace Yueby.EditorWindowExtends.HierarchyExtends
{
    [InitializeOnLoad]
    public class HierarchyExtender : EditorExtender<HierarchyExtender, HierarchyDrawer>
    {
        public override string Name => "Hierarchy";

        private Dictionary<int, SelectionItem> _selectionItems = new();

        public static HierarchyExtender Instance { get; private set; }

        public ActiveObjectHandler ActiveObjectHandler { get; set; } = null;

        private EditorWindow _lastWindow;

        static HierarchyExtender()
        {
            Instance = new HierarchyExtender();
        }

        public HierarchyExtender()
        {
            SetEnable(IsEnabled);
        }

        public override void SetEnable(bool value)
        {
            if (!value)
            {
                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemGUI;
                EditorApplication.update -= OnUpdate;
            }
            else
            {
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemGUI;
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;

                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyChanged += OnHierarchyChanged;

                EditorApplication.update -= OnUpdate;
                EditorApplication.update += OnUpdate;
            }

            base.SetEnable(value);
        }

        private void OnHierarchyChanged()
        {
            if (!Instance.IsEnabled)
                return;
            if (Instance is { ExtenderDrawers: null })
                return;

            foreach (var drawer in ExtenderDrawers.Where(d => d is { IsVisible: true }))
            {
                drawer.OnHierarchyChanged();
            }
        }

        private void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            if (!Instance.IsEnabled)
                return;
            if (Instance is { ExtenderDrawers: null })
                return;

            var selectionItem = GetSelectionItem(instanceID, selectionRect);
            foreach (var drawer in ExtenderDrawers.Where(d => d is { IsVisible: true }))
            {
                drawer.OnHierarchyWindowItemGUI(selectionItem);
            }
        }

        private void OnUpdate()
        {
            if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow != _lastWindow)
            {
                if (_lastWindow && _lastWindow.GetType().FullName == "UnityEditor.SceneHierarchyWindow")
                {
                    if (ActiveObjectHandler != null)
                    {
                        ActiveObjectHandler = null;
                    }
                }
                
                _lastWindow = EditorWindow.mouseOverWindow;
                // Logger.LogInfo(_lastWindow.GetType().FullName);
            }
        }

        private SelectionItem GetSelectionItem(int instanceID, Rect rect)
        {
            _selectionItems ??= new Dictionary<int, SelectionItem>();
            if (_selectionItems.TryGetValue(instanceID, out var selectionItem))
            {
                selectionItem.Refresh(instanceID, rect);
                return selectionItem;
            }

            selectionItem = new SelectionItem(instanceID, rect);
            _selectionItems.Add(instanceID, selectionItem);
            return selectionItem;
        }

        public void RemoveSelectionItem(int instanceID)
        {
            if (_selectionItems != null && _selectionItems.ContainsKey(instanceID))
            {
                _selectionItems.Remove(instanceID);
            }
        }

        public override void Repaint()
        {
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}

public class ActiveObjectHandler
{
    public bool Active;
    public GameObject LastGameObject;
}