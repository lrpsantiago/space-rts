using System;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts.FogOfWar
{
    public class FogOfWarThread
    {
        private Color[] _colors;
        private readonly IList<Vector2> _uvPos;
        private readonly IList<float> _radius;
        private readonly int _textureWidth;
        private readonly int _textureHeight;

        public FogOfWarThread(Color[] colors, IList<Vector2> uvPos, int textureWidth, int textureHeight, IList<float> radius)
        {
            _colors = colors;
            _uvPos = uvPos;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
            _radius = radius;
        }

        public void Run()
        {
            DoShit();
        }

        private void DoShit()
        {
            for (int i = 0; i < _uvPos.Count; i++)
            {
                var uv = _uvPos[i];
                var radius = _radius[i];
                var centerX = uv.x * _textureWidth - 0.5f;
                var centerY = uv.y * _textureHeight - 0.5f;
                var startX = Mathf.CeilToInt(centerX - radius);
                var startY = Mathf.CeilToInt(centerY - radius);
                var endX = Mathf.CeilToInt(centerX + radius);
                var endY = Mathf.CeilToInt(centerY + radius);
                var radiusSqr = radius * radius;
                var halfRadius = Mathf.Pow(radius / 2, 2);

                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        if (x < 0 || x >= _textureWidth
                            || y < 0 || y >= _textureHeight)
                        {
                            continue;
                        }

                        var color = _colors[y * _textureWidth + x];

                        if (color.a <= 0)
                        {
                            continue;
                        }

                        var dX = Mathf.Abs(x - centerX);
                        var dY = Mathf.Abs(y - centerY);
                        var dist = dX * dX + dY * dY;

                        if (dist > radiusSqr)
                        {
                            continue;
                        }

                        var alpha = (dist > halfRadius)
                            ? ((dist / halfRadius) - 1)
                            : 0;

                        _colors[y * _textureWidth + x].a = Mathf.Min(color.a, alpha);
                    }
                }
            }
        }
    }
}
