using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Yueby.Core.Utils;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtender<TExtender, TDrawer> : IEditorExtender where TExtender : EditorExtender<TExtender, TDrawer>, new() where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        public const string BaseMenuPath = "Tools/YuebyTools/Editor Window Extends/";
        public virtual string Name => GetType().FullName;
        protected List<TDrawer> ExtenderDrawers = new();

        public List<IEditorExtenderDrawer> Drawers
        {
            get
            {
                var result = ExtenderDrawers.ConvertAll(drawer => (IEditorExtenderDrawer)drawer);

                return result;
            }
            set
            {
                ExtenderDrawers = value.ConvertAll(drawer => (TDrawer)drawer);
                SortByIndex();
            }
        }

        // protected readonly ExtenderOptionModalWindowDrawer<TExtender, TDrawer> OptionModalDrawer;

        public bool IsEnabled
        {
            get => EditorPrefs.GetBool($"{Name}.IsEnabled", true);
            protected set
            {
                EditorPrefs.SetBool($"{Name}.IsEnabled", value);
                foreach (var drawer in ExtenderDrawers)
                {
                    drawer.ChangeVisible(value);
                }

                Repaint();
            }
        }

        protected EditorExtender() // 注意构造函数名称
        {
            InitializeDrawers();
            // OptionModalDrawer = new ExtenderOptionModalWindowDrawer<TExtender, TDrawer>(ExtenderDrawers, this);
        }

        private void SortByIndex()
        {
            ExtenderDrawers.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        private IEnumerable<Type> GetAllDrawerTypes()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == typeof(TDrawer))
                {
                    yield return type;
                }
            }
        }

        // public void ToggleEnable()
        // {
        //     SetEnable(!IsEnabled);
        // }

        public virtual void SetEnable(bool value)
        {
            if (IsEnabled == value) return;

            using (PerformanceTracker.Track("SetEnable"))
            {
                IsEnabled = value;
                if (value)
                    OnEnable();
                else
                    OnDisable();
                
                Repaint();
            }
        }

        public virtual void Repaint()
        {
        }

        public List<TDrawer> GetOrderedDrawers()
        {
            var drawers = ExtenderDrawers;
            drawers.Sort((a, b) => a.Order.CompareTo(b.Order));
            return drawers;
        }

        // protected virtual void ShowOptions()
        // {
        //     ModalEditorWindow.ShowUtility(OptionModalDrawer, showFocusCenter: false);
        // }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        private Dictionary<Type, TDrawer> _drawerCache = new();

        protected virtual void InitializeDrawers()
        {
            try
            {
                foreach (var drawerType in GetAllDrawerTypes())
                {
                    if (_drawerCache.ContainsKey(drawerType)) continue;

                    var drawer = (TDrawer)Activator.CreateInstance(drawerType);
                    drawer?.Init((TExtender)this);
                    ExtenderDrawers.Add(drawer);
                    _drawerCache[drawerType] = drawer;
                }

                SortByIndex();
            }
            catch (Exception ex)
            {
                YuebyLogger.LogException(ex, "Failed to initialize drawers");
            }
        }

        protected virtual void UpdateDrawers()
        {
            if (!IsEnabled) return;
            foreach (var drawer in ExtenderDrawers.Where(d => d.IsVisible))
            {
                drawer.OnUpdate();
            }
        }


        private readonly HashSet<Type> _activeDrawerTypes = new();


        protected readonly EditorPerformanceTracker PerformanceTracker = new();


        protected virtual string ConfigPath => $"EditorExtender.{Name}";

        protected T GetConfig<T>(string key, T defaultValue = default) =>
            EditorPrefs.HasKey($"{ConfigPath}.{key}") ? JsonUtility.FromJson<T>(EditorPrefs.GetString($"{ConfigPath}.{key}")) : defaultValue;

        protected void SetConfig<T>(string key, T value) =>
            EditorPrefs.SetString($"{ConfigPath}.{key}", JsonUtility.ToJson(value));

        // 5. 添加绘制器查找方法
        public TDrawer GetDrawer<T>() where T : TDrawer
        {
            return _drawerCache.Values.FirstOrDefault(d => d is T);
        }

        // 6. 添加批量操作方法
        public void EnableDrawers(params Type[] drawerTypes)
        {
            foreach (var type in drawerTypes)
            {
                if (_drawerCache.TryGetValue(type, out var drawer))
                {
                    drawer.ChangeVisible(true);
                    _activeDrawerTypes.Add(type);
                }
            }

            Repaint();
        }
    }
}