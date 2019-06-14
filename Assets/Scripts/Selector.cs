using PushingBoxStudios.Controls;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceRts
{
    public class Selector : MonoBehaviour
    {
        [SerializeField]
        private Color _selectorBorderColor = Utils.ToFloatRGBA(0, 255, 170);

        [SerializeField]
        private Color _selectorBackgroundColor = Utils.ToFloatRGBA(0, 255, 170, 32);

        [SerializeField]
        private int _selectorBorderThickness = 1;

        private Object _selectionPrefab;

        private Vector2 _selectionStart;

        private Vector2 _selectionEnd;

        public IList<ISelectableObject> SelectedObjects { get; set; }

        private void Awake()
        {
            _selectionPrefab = Resources.Load("Prefabs/Selection");
            SelectedObjects = new List<ISelectableObject>();
        }

        private void Update()
        {
            if (Input.GetMouseButton((int)EMouseButton.LeftButton))
            {
                if (Input.GetMouseButtonDown((int)EMouseButton.LeftButton))
                {
                    _selectionStart = Input.mousePosition;

                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        foreach (var o in SelectedObjects)
                        {
                            o.OnDismiss();
                        }

                        SelectedObjects.Clear();
                    }

                    PickSelectableObject();
                }
            }
            else if (Input.GetMouseButtonUp((int)EMouseButton.LeftButton))
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

            if (Input.GetMouseButtonDown((int)EMouseButton.RightButton))
            {
                var pickedPos = PickPosition();

                if (pickedPos != null)
                {
                    var pos = pickedPos.Value;

                    pos.x = Mathf.Round(pos.x);
                    pos.y = Mathf.Round(pos.y);
                    pos.z = Mathf.Round(pos.z);

                    foreach (var o in SelectedObjects)
                    {
                        o.OnPositionSelection(pos);
                    }
                }
            }
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

        private Vector3? PickPosition()
        {
            return ProjectScreenPosition(Input.mousePosition);
        }

        private Vector3? ProjectScreenPosition(Vector3 position)
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

            return null;
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
