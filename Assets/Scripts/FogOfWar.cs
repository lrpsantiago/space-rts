using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class FogOfWar : MonoBehaviour
    {
        [SerializeField]
        private GameObject _fogOfWarPlane;

        [SerializeField]
        private Transform _player;

        [SerializeField]
        private LayerMask _fogLayer;

        [SerializeField]
        private float _radius = 5f;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private Color[] _colors;

        private float RadiusSqr => _radius * _radius;

        private IList<Transform> _units;

        private void Start()
        {
            _mesh = _fogOfWarPlane.GetComponent<MeshFilter>().mesh;
            _vertices = _mesh.vertices;
            _colors = new Color[_vertices.Length];

            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Color.black;
            }

            UpdateColor();

            _units = new List<Transform>();
            Invoke("UpdateUnitList", 1);
        }

        private void Update()
        {
            //for (int j = 0; j < _units.Count; j++)
            {
                var unit = _player; //_units[j];
                var ray = new Ray(transform.position, unit.position - transform.position);

                if (Physics.Raycast(ray, out RaycastHit hit, 1000, _fogLayer, QueryTriggerInteraction.Collide))
                {
                    for (int i = 0; i < _vertices.Length; i++)
                    {
                        var vertexPosition = _fogOfWarPlane.transform
                            .TransformPoint(_vertices[i]);

                        var dist = Vector3.SqrMagnitude(vertexPosition - hit.point);

                        if (dist < RadiusSqr)
                        {
                            var alpha = (dist < RadiusSqr / 2)
                                ? 0
                                : Mathf.Min(_colors[i].a, (dist - (RadiusSqr / 2)) / (RadiusSqr / 2));
                            _colors[i].a = alpha;
                        }
                    }

                    UpdateColor();
                }
            }
        }

        private void UpdateColor()
        {
            _mesh.colors = _colors;
        }

        private void UpdateUnitList()
        {
            var units = GameObject.FindGameObjectsWithTag("Unit");

            foreach (var item in units)
            {
                if (_units.Contains(item.transform))
                {
                    continue;
                }

                _units.Add(item.transform);
            }

            Invoke("UpdateUnitList", 3);
        }
    }
}
