using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.FogOfWar
{
    public class FogOfWar : MonoBehaviour, IFogOfWar
    {
        [SerializeField]
        private GameObject _fogCamera;

        [SerializeField]
        private GameObject _mainFogPlane;

        [SerializeField]
        private LayerMask _fogLayer;

        [SerializeField]
        private int _textureWidth = 100;

        [SerializeField]
        private int _textureHeight = 100;

        private Texture2D _texture;
        private Color[] _colors;
        private IList<FogOfWarViewer> _viewers;
        private bool _isOdd;
        private bool _revealed;
        private IList<int> _lastPos;
        private IList<Vector2> _hitsUvPos;
        private IList<float> _viewersRadius;
        private Thread thread;
        private DateTime _lastTimeStamp;

        private void Awake()
        {
            _viewers = new List<FogOfWarViewer>();
            _texture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.Alpha8, false);
            _colors = new Color[_textureWidth * _textureHeight];

            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.black;
            }

            _mainFogPlane.GetComponent<MeshRenderer>().material.mainTexture = _texture;
            _lastPos = new List<int>();
            _hitsUvPos = new List<Vector2>();
            _viewersRadius = new List<float>();

            UpdateTexture();
        }

        private void Update()
        {
            if (_revealed)
            {
                return;
            }

            var now = DateTime.Now;
            var diff = now - _lastTimeStamp;

            if (diff.TotalMilliseconds < 50)
            {
                return;
            }

            _lastTimeStamp = now;
            _hitsUvPos.Clear();
            _viewersRadius.Clear();

            for (int i = 0; i < _viewers.Count; i++)
            {
                var mod = i % 2;

                if (_isOdd && mod == 0 || !_isOdd && mod == 1)
                {
                    continue;
                }

                var position = _viewers[i].Transform.position;

                if (_lastPos[i] == position.GetHashCode())
                {
                    continue;
                }

                _lastPos[i] = position.GetHashCode();

                var radius = _viewers[i].ViewRadius;
                var origin = _fogCamera.transform.position;
                var direction = position - _fogCamera.transform.position;

                if (!Physics.Raycast(origin, direction, out RaycastHit hit, float.PositiveInfinity, _fogLayer))
                {
                    continue;
                }

                _hitsUvPos.Add(hit.textureCoord);
                _viewersRadius.Add(radius);
            }

            _isOdd = !_isOdd;

            var fogThread = new FogOfWarThread(_colors, _hitsUvPos, _textureWidth, _textureHeight, _viewersRadius);
            thread = new Thread(fogThread.Run);
            thread.Start();

            UpdateTexture();
        }

        public void RevealPosition(Vector3 position, float radius)
        {
            var origin = _fogCamera.transform.position;
            var direction = position - _fogCamera.transform.position;

            if (!Physics.Raycast(origin, direction, out RaycastHit hit, float.PositiveInfinity, _fogLayer))
            {
                return;
            }

            var uv = hit.textureCoord;
            var centerX = (uv.x * _textureWidth) - 0.5f;
            var centerY = (uv.y * _textureHeight) - 0.5f;
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

        public void RevealAll()
        {
            _mainFogPlane.SetActive(false);
            _revealed = true;
        }

        public void AddViewer(Transform viewerTransform, float radius = 20f)
        {
            var viewer = new FogOfWarViewer
            {
                Transform = viewerTransform,
                ViewRadius = radius
            };

            _viewers.Add(viewer);
            _lastPos.Add(viewerTransform.position.GetHashCode());
            RevealPosition(viewer.Transform.position, radius);
        }

        public void UpdateViewerRadius(Transform transform, float newRadius)
        {
            var viewer = _viewers.FirstOrDefault(v => v.Transform == transform);

            if (viewer == null)
            {
                return;
            }

            viewer.ViewRadius = newRadius;
        }
        
        public void RemoveViewer(Transform viewerTransform)
        {
            var viewer = _viewers.SingleOrDefault(v => v.Transform == viewerTransform);

            if (viewer == null)
            {
                return;
            }

            var index = _viewers.IndexOf(viewer);

            _lastPos.RemoveAt(index);
            _viewers.Remove(viewer);
        }

        public void HideAll()
        {
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.black;
            }

            UpdateTexture();
        }

        private void UpdateTexture()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
        }
    }
}
