using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public class FogOfWar : MonoBehaviour
    {
        [SerializeField]
        private GameObject _fogOfWarPlane;

        [SerializeField]
        private LayerMask _fogLayer;

        [SerializeField]
        private int _textureWidth = 100;

        [SerializeField]
        private int _textureHeight = 100;

        private Texture2D _texture;
        private Color[] _colors;
        private IList<FogOfWarViewer> _viewers;
        private Vector3 vectorCenter = new Vector3();
        private Vector3 vectorPos = new Vector3();

        private void Awake()
        {
            _viewers = new List<FogOfWarViewer>();
            _texture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.Alpha8, false);
            _colors = new Color[_textureWidth * _textureHeight];

            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.black;
            }

            _fogOfWarPlane.GetComponent<MeshRenderer>().material.mainTexture = _texture;

            UpdateColor();
        }

        private void Update()
        {
            for (int i = 0; i < _viewers.Count; i++)
            {
                var position = _viewers[i].Transform.position;
                var radius = _viewers[i].ViewRadius;

                RevealPosition(position, radius);
            }

            UpdateColor();
        }

        public void RevealPosition(Vector3 position, float radius)
        {
            var direction = position - transform.position;
            var ray = new Ray(transform.position, direction);

            if (!Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _fogLayer))
            {
                return;
            }

            var uv = hit.textureCoord;
            var centerX = Mathf.RoundToInt(uv.x * _textureWidth);
            var centerY = Mathf.RoundToInt(uv.y * _textureHeight);
            var startX = Mathf.RoundToInt(centerX - radius);
            var startY = Mathf.RoundToInt(centerY - radius);
            var endX = Mathf.RoundToInt(centerX + radius);
            var endY = Mathf.RoundToInt(centerY + radius);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    if (x < 0 || x > _textureWidth
                        || y < 0 || y > _textureHeight)
                    {
                        continue;
                    }

                    var color = _colors[y * _textureWidth + x];

                    if (color.a <= 0)
                    {
                        continue;
                    }

                    vectorCenter.Set(centerX, 0, centerY);
                    vectorPos.Set(x, 0, y);

                    var dist = Vector3.SqrMagnitude(vectorPos - vectorCenter);
                    var halfDist = Mathf.Pow(radius / 2, 2);

                    if (dist > radius * radius)
                    {
                        continue;
                    }

                    var alpha = dist > halfDist
                        ? ((dist - halfDist) / halfDist) * 0.25f
                        : 0;

                    _colors[y * _textureWidth + x].a = Mathf.Min(color.a, alpha);
                }
            }
        }

        public void AddViewer(Transform viewerTransform, float radius = 10f)
        {
            var viewer = new FogOfWarViewer
            {
                Transform = viewerTransform,
                ViewRadius = radius
            };

            _viewers.Add(viewer);
        }

        public void RemoveViewer(Transform viewerTransform)
        {
            var viewer = _viewers.SingleOrDefault(v => v.Transform == viewerTransform);

            if (viewer == null)
            {
                return;
            }

            _viewers.Remove(viewer);
        }
        
        private void UpdateColor()
        {
            _texture.SetPixels(_colors);
            _texture.Apply();
        }
    }
}
