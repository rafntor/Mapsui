﻿using System;
using System.Collections.Generic;
using Mapsui.Styles;
using SkiaSharp;

namespace Mapsui.Rendering.Skia.Cache
{
    public class VectorCache : IVectorCache
    {
        private readonly Dictionary<(Pen? Pen, float Opacity), object> _paintCache = new();
        private readonly Dictionary<(Brush? Brush, float Opacity, double rotation), object> _fillCache = new();
        private readonly Dictionary<(MRect Rect, double Resolution, object Geometry, float lineWidth), object> _pathCache = new();
        private readonly ISymbolCache _symbolCache;

        public VectorCache(ISymbolCache? symbolCache)
        {
            _symbolCache = symbolCache;
        }

        public T GetOrCreatePaint<T>(Pen? pen, float opacity, Func<Pen?, float, T> toPaint) where T : class
        {
            var key = (pen, opacity);
            if (!_paintCache.TryGetValue(key, out var paint))
            {
                paint = toPaint(pen, opacity);
                _paintCache[key] = paint;
            }

            return (T)paint;
        }

        public T GetOrCreatePaint<T>(Brush? pen, float opacity, double rotation, Func<Brush?, float, double, ISymbolCache, T> toPaint) where T : class
        {
            var key = (pen, opacity, rotation);
            if (!_fillCache.TryGetValue(key, out var paint))
            {
                paint = toPaint(pen, opacity, rotation, _symbolCache);
                _fillCache[key] = paint;
            }

            return (T)paint;
        }

        public TPath GetOrCreatePath<TPath, TGeometry>(IReadOnlyViewport viewport, TGeometry geometry, float lineWidth, Func<TGeometry, IReadOnlyViewport, float, TPath> toPath) where TPath : class where TGeometry : class
        {
            var key = (viewport.Extent, viewport.Rotation, geometry, lineWidth);
            if (!_pathCache.TryGetValue(key, out var path))
            {
                path = toPath(geometry, viewport, lineWidth);
                _pathCache[key] = path;
            }

            return (TPath)path;
        }
    }
}