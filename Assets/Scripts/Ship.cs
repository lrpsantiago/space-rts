using PushingBoxStudios;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRts
{
    public class Ship : MonoBehaviour, ISelectableObject
    {
        private static GameObject _unitCanvasPrefab;

        [SerializeField]
        private float _acceleration = 0.2f;

        [SerializeField]
        private float _deceleration = 0.25f;

        [SerializeField]
        private float _movementSpeedMax = 2;

        private float _movementSpeed = 0;

        [SerializeField]
        private float _angularSpeed = 0;

        private IList<Vector3> _destinationQueue;
        private Gauge _hpGauge = new Gauge(100);

        public event EventHandler HpChanged
        {
            add
            {
                _hpGauge.CurrentValueChanged += value;
            }

            remove
            {
                _hpGauge.CurrentValueChanged -= value;
            }
        }

        public event EventHandler Killed;
        public event EventHandler Destructed;

        public bool IsSelected { get; set; }

        private void Awake()
        {
            if (_unitCanvasPrefab == null)
            {
                _unitCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/PFB_UnitHpBar");
            }
        }

        private void Start()
        {
            var hpBarObj = Instantiate(_unitCanvasPrefab);

            if (hpBarObj != null)
            {
                var parent = GameObject.Find("Canvas/HpBarsUI").transform;
                var hpBar = hpBarObj.GetComponent<UnitHpBar>();

                hpBar.Initialize(this);
                hpBar.transform.SetParent(parent, false);
            }

            _destinationQueue = new List<Vector3>();
            _movementSpeed = _movementSpeedMax;
        }

        private void Update()
        {
            if (_destinationQueue.Count > 0)
            {
                var destinationPos = _destinationQueue[0];

                UpdateRotation(destinationPos);

                var pos = transform.position;
                var distance = Vector3.Distance(pos, destinationPos);

                if (_destinationQueue.Count > 1)
                {
                    float sum = distance;

                    for (int i = 0; i < _destinationQueue.Count - 1; i++)
                    {
                        var p1 = _destinationQueue[i];
                        var p2 = _destinationQueue[i + 1];

                        sum += Vector3.Distance(p1, p2);
                    }

                    distance = sum;
                }

                var decelDistance = CalculateDecelDistance();
                var variant = _movementSpeed + _acceleration * Time.deltaTime;

                if (distance <= decelDistance)
                {
                    variant = _movementSpeed - _deceleration * Time.deltaTime;
                }

                _movementSpeed = Mathf.Clamp(variant, 0, _movementSpeedMax);

                var velocity = _movementSpeed * Time.deltaTime;
                pos = Vector3.MoveTowards(pos, destinationPos, velocity);

                transform.position = pos;
                distance = Vector3.Distance(pos, destinationPos);

                if (transform.position == destinationPos)
                {
                    _destinationQueue.RemoveAt(0);
                }
            }
            else
            {
                _movementSpeed = 0;
            }
        }

        private double CalculateDecelDistance()
        {
            var distance = Mathf.Pow(_movementSpeed, 2) / (2 * _deceleration);

            return distance;
        }

        private void UpdateRotation(Vector3 targetPos)
        {
            var dir = targetPos - transform.position;
            var toRot = Quaternion.LookRotation(dir, Vector3.up);
            var fromRot = transform.rotation;
            var rot = Quaternion.RotateTowards(fromRot, toRot, _angularSpeed * Time.deltaTime);

            transform.rotation = rot;
        }

        public void OnSelection()
        {
            IsSelected = true;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);
        }

        public void OnDismiss()
        {
            IsSelected = false;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);
        }

        public void OnPositionSelection(Vector3 position)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                _destinationQueue.Clear();
            }

            _destinationQueue.Add(position);
        }
    }
}