using PushingBoxStudios.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceRts
{
    public class ObjectSelector : MonoBehaviour
    {
        private static GameObject _positionSelectionPrefab;
        private static GameObject _unitSelectionPrefab;

        [SerializeField]
        private Color _selectorBorderColor = Utils.ToFloatRGBA(0, 255, 170);

        [SerializeField]
        private Color _selectorBackgroundColor = Utils.ToFloatRGBA(0, 255, 170, 32);

        [SerializeField]
        private int _selectorBorderThickness = 1;

        private Vector2 _selectionStart;

        private Vector2 _selectionEnd;

        private IList<GameObject> _positionSelectionInstances;

        public IList<ISelectableObject> SelectedObjects { get; set; }

        private void Awake()
        {
            if (_unitSelectionPrefab == null)
            {
                _unitSelectionPrefab = Resources.Load<GameObject>("Prefabs/Selection");
            }

            if (_positionSelectionPrefab == null)
            {
                _positionSelectionPrefab = Resources.Load<GameObject>("Prefabs/PositionSelection");
            }

            _positionSelectionInstances = new List<GameObject>();

            SelectedObjects = new List<ISelectableObject>();
        }

        private void Update()
        {
            if (Input.GetMouseButton((int)EMouseButton.LeftButton)
                && Input.GetMouseButtonDown((int)EMouseButton.LeftButton))
            {
                StartSelection();
            }
            else if (Input.GetMouseButtonUp((int)EMouseButton.LeftButton))
            {
                EndSelection();
            }

            if (SelectedObjects.Count <= 0)
            {
                return;
            }

            if (Input.GetMouseButton((int)EMouseButton.RightButton))
            {
                var obj = PickObjectIfExists(Input.mousePosition);

                if (obj != null)
                {
                    foreach (var o in SelectedObjects)
                    {
                        o.OnPointToAnotherObject(obj);
                    }

                    return;
                }

                var position = ProjectScreenPosition(Input.mousePosition);
                var roundedPointedPos = position.Round();

                if (Input.GetMouseButtonDown((int)EMouseButton.RightButton))
                {
                    var columns = Mathf.CeilToInt(Mathf.Sqrt(SelectedObjects.Count));
                    var lines = Mathf.CeilToInt(Mathf.Sqrt(SelectedObjects.Count));

                    for (int i = 0; i < SelectedObjects.Count; i++)
                    {
                        var o = SelectedObjects[i];
                        var p = roundedPointedPos;

                        p.x += i % columns;
                        p.z += (i / columns) % lines;

                        var instance = Instantiate(_positionSelectionPrefab, p, Quaternion.identity);
                        _positionSelectionInstances.Add(instance);
                    }
                }

                var posSelection = _positionSelectionInstances.FirstOrDefault();

                if (posSelection != null)
                {
                    var roundedSelectionPos = posSelection.transform.position.Round();

                    foreach (var s in _positionSelectionInstances)
                    {
                        var arrowObj = s.transform
                            .Find("Arrow")
                            .gameObject;

                        if (roundedPointedPos != roundedSelectionPos)
                        {
                            var pointed = ((roundedPointedPos - roundedSelectionPos) + s.transform.position)
                            .Round();

                            s.transform.LookAt(pointed);

                            arrowObj.SetActive(true);
                            continue;
                        }

                        arrowObj.SetActive(false);
                    }
                }
            }
            else if (Input.GetMouseButtonUp((int)EMouseButton.RightButton))
            {
                if (_positionSelectionInstances.Count <= 0)
                {
                    return;
                }

                var originalPosSelction = _positionSelectionInstances.First()
                    .transform
                    .position;

                for (int i = 0; i < SelectedObjects.Count; i++)
                {
                    var obj = SelectedObjects[i];
                    var posSelection = _positionSelectionInstances[i];
                    var pos = posSelection.transform.position;
                    var arrowObj = posSelection.transform
                            .Find("Arrow")
                            .gameObject;

                    Vector3? lookDir = null;

                    if (arrowObj.activeInHierarchy)
                    {
                        lookDir = posSelection.transform.forward;
                    }

                    obj.OnPositionSelection(pos, lookDir);
                }

                foreach (var s in _positionSelectionInstances)
                {
                    Destroy(s);
                }

                _positionSelectionInstances.Clear();
            }
        }

        private void EndSelection()
        {
            if (_selectionStart != null)
            {
                _selectionEnd = Input.mousePosition;
            }

            var objects = FindObjectsOfType<MonoBehaviour>()
                .Where(o => o is ISelectableObject);

            foreach (var o in objects)
            {
                var selectionBounds = GetSelectionBounds();
                var objPos = Camera.main.WorldToViewportPoint(o.transform.position);

                if (selectionBounds.Contains(objPos))
                {
                    var selectableObj = o.GetComponent<ISelectableObject>();

                    if (selectableObj != null && !SelectedObjects.Contains(selectableObj))
                    {
                        SelectedObjects.Add(selectableObj);
                        selectableObj.OnSelection();
                    }
                }
            }
        }

        private void StartSelection()
        {
            _selectionStart = Input.mousePosition;

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (var o in SelectedObjects)
                {
                    o.OnSelectionDismiss();
                }

                SelectedObjects.Clear();
            }

            PickSelectableObject();
        }

        private ISelectableObject PickObjectIfExists(Vector3 position)
        {
            var ray = Camera.main.ScreenPointToRay(position);
            var layers = new string[] { "Selectable" };
            var mask = LayerMask.GetMask(layers);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity, mask);

            if (hits.Length > 0)
            {
                var hit = hits[0];
                Debug.DrawLine(ray.origin, hit.point);

                var selectableObj = hit.collider
                    .gameObject
                    .GetComponent<ISelectableObject>();

                return selectableObj;
            }

            return null;
        }

        void OnGUI()
        {
            if (Input.GetMouseButton((int)EMouseButton.LeftButton))
            {
                var rect = Utils.GetScreenRect(_selectionStart, Input.mousePosition);

                Utils.DrawScreenRect(rect, _selectorBackgroundColor);
                Utils.DrawScreenRectBorder(rect, _selectorBorderThickness, _selectorBorderColor);
            }
        }

        private Vector3 PickPosition()
        {
            return ProjectScreenPosition(Input.mousePosition);
        }

        private Vector3 ProjectScreenPosition(Vector3 position)
        {
            var ray = Camera.main.ScreenPointToRay(position);
            var layers = new string[] { "Grid" };
            var mask = LayerMask.GetMask(layers);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity, mask);

            if (hits.Length > 0)
            {
                var hit = hits[0];
                Debug.DrawLine(ray.origin, hit.point);

                if (hit.collider.tag == "Grid")
                {
                    return hit.point;
                }
            }

            throw new Exception("Grid not defined");
        }

        private Bounds GetSelectionBounds()
        {
            var v1 = Camera.main.ScreenToViewportPoint(_selectionStart);
            var v2 = Camera.main.ScreenToViewportPoint(_selectionEnd);
            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);

            min.z = Camera.main.nearClipPlane;
            max.z = Camera.main.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }


        private void PickSelectableObject()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layers = new string[] { "Selectable" };
            var mask = LayerMask.GetMask(layers);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity, mask);

            if (hits.Length > 0)
            {
                var hit = hits[0];
                Debug.DrawLine(ray.origin, hit.point);

                var selectableObj = hit.collider
                    .gameObject
                    .GetComponent<ISelectableObject>();

                if (selectableObj != null && !SelectedObjects.Contains(selectableObj))
                {
                    SelectedObjects.Add(selectableObj);
                    selectableObj.OnSelection();
                }
            }
        }
    }
}
