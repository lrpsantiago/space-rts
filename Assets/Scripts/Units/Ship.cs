using PushingBoxStudios;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceRts
{
    public class Ship : StateMachine, ISelectableObject
    {
        #region Fields

        private static GameObject _unitCanvasPrefab;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _description;

        [SerializeField]
        private float _acceleration = 1;

        [SerializeField]
        private float _deceleration = 2;

        [SerializeField]
        private float _movementSpeedMax = 3;

        [SerializeField]
        private float _stationaryAngularSpeed = 60;

        private readonly float _angularSpeed = 360;
        private Gauge _hpGauge = new Gauge(100);
        private UnitProgressBar _hpBar;

        #endregion


        #region Events

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

        #endregion


        #region Properties

        internal IState StationaryState { get; set; }

        internal IState MovingState { get; set; }

        public bool IsSelected { get; set; }

        public float Acceleration
        {
            get { return _acceleration; }
        }

        public float Deceleration
        {
            get
            {
                return _deceleration;
            }
        }

        public float MovementSpeed { get; internal set; }

        public float MovementSpeedMax
        {
            get
            {
                return _movementSpeedMax;
            }
        }

        public float AngularSpeed
        {
            get
            {
                return _angularSpeed;
            }
        }

        internal IList<Vector3> DestinationQueue { get; set; }

        public float StationaryAngularSpeed
        {
            get
            {
                return _stationaryAngularSpeed;
            }
        }

        public Vector3? DestinationFacingDirection { get; set; }

        public Transform Transform
        {
            get { return transform; }
        }

        #endregion

        protected void Awake()
        {
            if (_unitCanvasPrefab == null)
            {
                _unitCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/PFB_UnitHpBar");
            }

            StationaryState = new StationaryShipState(this);
            MovingState = new MovingShipState(this);
            DestinationQueue = new List<Vector3>();
            DestinationFacingDirection = transform.forward;

            CurrentState = StationaryState;
        }

        protected void Start()
        {
            var hpBarObj = Instantiate(_unitCanvasPrefab);

            if (hpBarObj != null)
            {
                var parent = GameObject.Find("Canvas/HpBarsUI").transform;
                var hpBar = hpBarObj.GetComponent<UnitProgressBar>();

                hpBar.LowestValueColor = Utils.ToFloatRGBA(0, 255, 170);
                hpBar.HighestValueColor = Utils.ToFloatRGBA(0, 255, 170);
                hpBar.FloatingHeight = 0.5f;

                hpBar.Initialize(gameObject, _hpGauge);
                hpBar.transform.SetParent(parent, false);
                _hpBar = hpBar;
            }
        }

        protected void OnDestroy()
        {
            if (Destructed != null)
            {
                Destructed(this, EventArgs.Empty);
            }
        }

        internal void UpdateRotation(Vector3 targetPos, float angularSpeed)
        {
            var dir = targetPos - transform.position;
            var toRot = Quaternion.LookRotation(dir, Vector3.up);
            var fromRot = transform.rotation;
            var rot = Quaternion.RotateTowards(fromRot, toRot, angularSpeed * Time.deltaTime);

            transform.rotation = rot;
        }

        internal void UpdateRotationDirection(Vector3 targetPos, float angularSpeed)
        {
            var dir = targetPos - transform.position;
            var toRot = Quaternion.LookRotation(dir, Vector3.up);
            var fromRot = transform.rotation;
            var rot = Quaternion.RotateTowards(fromRot, toRot, angularSpeed * Time.deltaTime);

            transform.rotation = rot;
        }

        public void OnSelection()
        {
            IsSelected = true;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);

            _hpBar.IsVisible = true;

            var titleUi = GameObject.Find("TitleGroup/Title")
                .GetComponent<Text>();

            var subtitleUi = GameObject.Find("TitleGroup/Subtitle")
                .GetComponent<Text>();

            titleUi.text = _name;
            subtitleUi.text = _description;
        }

        public void OnSelectionDismiss()
        {
            IsSelected = false;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);

            _hpBar.IsVisible = false;

            var titleUi = GameObject.Find("TitleGroup/Title")
                .GetComponent<Text>();

            var subtitleUi = GameObject.Find("TitleGroup/Subtitle")
                .GetComponent<Text>();

            titleUi.text = string.Empty;
            subtitleUi.text = string.Empty;
        }

        public void OnPositionSelection(Vector3 position, Vector3? facingDirection)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DestinationQueue.Clear();
            }

            DestinationQueue.Add(position);
            DestinationFacingDirection = facingDirection;
        }

        public void OnPointToAnotherObject(ISelectableObject anotherObject)
        {
            if (anotherObject is Ship)
            {
                Debug.Log("Another ship.");
            }
            else if (anotherObject is Planet)
            {
                Debug.Log("Planet " + ((Planet)anotherObject).Name);
            }
        }

        public void TakeDamage(float amount)
        {
            _hpGauge.CurrentValue -= amount;
        }
    }
}