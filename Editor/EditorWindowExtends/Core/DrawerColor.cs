using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Yueby.EditorWindowExtends.Core
{
    public class DrawerColor
    {
        public Color LightColor { get; }
        public Color DarkColor { get; }

        // 1. 改进缓存机制
        private static readonly Dictionary<int, Color> _colorCache = new();
        private readonly int _colorHash;

        public DrawerColor(Color light, Color dark)
        {
            LightColor = light;
            DarkColor = dark;
            _colorHash = (light.GetHashCode() * 397) ^ dark.GetHashCode();
        }

        public Color GetColor()
        {
            if(_colorCache.TryGetValue(_colorHash, out var cachedColor))
                return cachedColor;
                
            var color = EditorGUIUtility.isProSkin ? DarkColor : LightColor;
            _colorCache[_colorHash] = color;
            return color;
        }

        // 2. 添加颜色混合
        public DrawerColor Blend(DrawerColor other, float t)
        {
            return new DrawerColor(
                Color.Lerp(LightColor, other.LightColor, t),
                Color.Lerp(DarkColor, other.DarkColor, t)
            );
        }
    }
}