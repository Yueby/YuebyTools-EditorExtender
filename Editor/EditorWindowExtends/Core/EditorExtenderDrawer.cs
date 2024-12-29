using System;
using UnityEditor;
using Yueby.Core.Utils;

namespace Yueby.EditorWindowExtends.Core
{
    public class EditorExtenderDrawer<TExtender, TDrawer> : IEditorExtenderDrawer
        where TExtender : EditorExtender<TExtender, TDrawer>, new()
        where TDrawer : EditorExtenderDrawer<TExtender, TDrawer>, new()
    {
        // 1. 添加绘制器状态
        protected bool _initialized;
        protected bool _enabled;

        // 2. 改进初始化流程
        public virtual void Init(TExtender extender)
        {
            if (_initialized)
                return;

            try
            {
                Extender = extender;
                OnInitialize();
                _initialized = true;
            }
            catch (Exception ex)
            {
                YuebyLogger.LogException(ex, $"Failed to initialize drawer: {GetType().Name}");
            }
        }

        protected virtual void OnInitialize()
        {
        }

        // 3. 添加资源清理
        public virtual void Cleanup()
        {
            _initialized = false;
            _enabled = false;
            OnCleanup();
        }

        protected virtual void OnCleanup()
        {
        }

        // 4. 添加性能监控
        private System.Diagnostics.Stopwatch _stopwatch = new();

        protected void BeginPerformanceCheck() => _stopwatch.Start();

        protected void EndPerformanceCheck(string operation)
        {
            _stopwatch.Stop();
            if (_stopwatch.ElapsedMilliseconds > 100)
                YuebyLogger.LogWarning(
                    $"{GetType().Name} {operation} took {_stopwatch.ElapsedMilliseconds}ms"
                );
            _stopwatch.Reset();
        }

        // 将 Extender 属性修改为 TExtender 类型
        public virtual TExtender Extender { get; set; }

        public string SavePath => $"{GetType().FullName}";

        public bool IsVisible
        {
            get
            {
                var isVisible = EditorPrefs.GetBool($"{SavePath}.IsVisible", true);

                return isVisible;
            }
        }

        public int Order => DefaultOrder;

        protected virtual int DefaultOrder { get; } = 0;
        public virtual string DrawerName { get; } = "";
        public virtual string Tooltip { get; } = "";

        public EditorExtenderDrawer()
        {
            DrawerName = GetType().Name;
        }

        public void ChangeVisible(bool value)
        {
            EditorPrefs.SetBool($"{SavePath}.IsVisible", value);
            Repaint();
        }


        public void Repaint()
        {
            Extender.Repaint();
        }

        public virtual void OnUpdate()
        {
        }
    }
}