using Assets.Scripts.FogOfWar;
using Assets.Scripts.Projectiles;
using Assets.Scripts.Units;
using PushingBoxStudios;
using SpaceRts.Planets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRts
{
    public class Ship : StateMachine, ISelectableObject
    {
        #region Fields

        private static GameObject _unitCanvasPrefab;
        private static GameObject _laserPrefab;

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

        [SerializeField]
        private float _visionRange = 10f;

        [SerializeField]
        private int _maxHp = 100;

        private readonly float _angularSpeed = 360;
        private GameObject _selection;
        private Gauge _hpGauge;
        private UnitProgressBar _hpBar;
        private IFogOfWar _fogOfWar;

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

        internal IState EnteringWormholeState { get; set; }

        internal IState ExitingWormholeState { get; set; }

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

        public string Name => _name;

        public string Description => _description;

        public Transform Transform => transform;

        public bool GoingToEnterWormhole { get; set; }

        public Ship Target { get; private set; }

        #endregion

        protected void Awake()
        {
            if (_unitCanvasPrefab == null)
            {
                _unitCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/PFB_UnitHpBar");
            }

            if (_laserPrefab == null)
            {
                _laserPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Missile");
            }

            StationaryState = new StationaryShipState(this);
            MovingState = new MovingShipState(this);
            EnteringWormholeState = new EnteringWormholeShipState(this);
            ExitingWormholeState = new ExitingWormholeShipState(this);

            DestinationQueue = new List<Vector3>();
            DestinationFacingDirection = transform.forward;

            CurrentState = StationaryState;
        }

        protected void Start()
        {
            _fogOfWar = GameObject.Find("FogOfWar")
                .GetComponent<IFogOfWar>();

            _fogOfWar.AddViewer(transform, _visionRange);

            _hpGauge = new Gauge(_maxHp);
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

            _selection = transform.Find("Selection")
                .gameObject;
        }

        protected override void Update()
        {
            base.Update();

            if (Target != null)
            {
                Shoot(Target);
            }
        }

        protected void OnDestroy()
        {
            var fog = GameObject.Find("FogCamera")
                .GetComponent<FogOfWar>();

            fog.RemoveViewer(transform);

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
            _selection.SetActive(IsSelected);
            _hpBar.IsVisible = true;
        }

        public void OnSelectionDismiss()
        {
            IsSelected = false;
            _selection.SetActive(IsSelected);
            _hpBar.IsVisible = false;
        }

        public void OnPositionSelection(Vector3 position, Vector3? facingDirection)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DestinationQueue.Clear();
            }

            DestinationQueue.Add(position);
            DestinationFacingDirection = facingDirection;
            Target = null;
        }

        public void OnPointToAnotherObject(ISelectableObject anotherObject)
        {
            if (anotherObject is Ship)
            {
                var otherShip = anotherObject.Transform.gameObject.GetComponent<Ship>();
                Target = otherShip;
            }
            else if (anotherObject is Planet planet)
            {
                Debug.Log("Planet " + planet.Name);
            }
            else if (anotherObject is Wormhole wormhole)
            {
                DestinationQueue.Clear();
                DestinationQueue.Add(wormhole.transform.position);
                DestinationFacingDirection = wormhole.transform.position;

                GoingToEnterWormhole = true;
            }
        }

        public void TakeDamage(float amount)
        {
            _hpGauge.CurrentValue -= amount;
        }

        private DateTime _shootTimeStamp;

        private void Shoot(Ship target)
        {
            var now = DateTime.Now;
            var diff = now - _shootTimeStamp;

            if (diff.TotalMilliseconds < 500)
            {
                return;
            }

            _shootTimeStamp = now;

            var gameObj = Instantiate(_laserPrefab, transform.position, transform.rotation);
            var laser = gameObj.GetComponent<Missile>();
            laser.Target = target;
        }
    }
}